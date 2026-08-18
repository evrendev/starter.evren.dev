<script setup lang="ts">
import { ref, computed, watch, nextTick } from "vue";
import { useRouter } from "vue-router";
import { usePayPalSdk } from "@/composables/usePayPalSdk";
import { usePaymentStore } from "@/stores/payment";
import { useCourseEnrollmentStore } from "@/stores/courseEnrollment";
import { Notify } from "@/stores/notification";

const props = defineProps<{
  modelValue: boolean;
  course: { courseId: string; title: string; amount: number };
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const { t } = useI18n();
const router = useRouter();
const { state: sdkState, load: loadPayPalSdk } = usePayPalSdk();
const paymentStore = usePaymentStore();
const enrollmentStore = useCourseEnrollmentStore();

const buttonsContainer = ref<HTMLElement | null>(null);
const rendering = ref(false);
const renderError = ref<string | null>(null);
// Remembers our own PaymentOrder.Id (not the PayPal order id PayPal's SDK
// hands back in onApprove) — the capture endpoint is keyed by ours, see
// PaymentsController.CaptureOrderAsync(Guid id).
let currentPaymentOrderId: string | null = null;

const open = computed({
  get: () => props.modelValue,
  set: (value) => emit("update:modelValue", value),
});

const renderButtons = async () => {
  rendering.value = true;
  renderError.value = null;

  try {
    await loadPayPalSdk();
    await nextTick();

    if (!buttonsContainer.value || !window.paypal) return;

    // A fresh render.() call is needed every time the dialog opens for a
    // (possibly different) course — clear any previous buttons first.
    buttonsContainer.value.innerHTML = "";

    window.paypal
      .Buttons({
        createOrder: async () => {
          const result = await paymentStore.createOrder(props.course.courseId);
          if (!result.succeeded || !result.data) {
            Notify.error(t("learning.checkout.error"));
            throw new Error("Failed to create PayPal order");
          }
          currentPaymentOrderId = result.data.paymentOrderId;
          return result.data.payPalOrderId;
        },
        onApprove: async (data: { orderID: string }) => {
          if (!currentPaymentOrderId) return;

          const result = await paymentStore.captureOrder(currentPaymentOrderId);
          if (result.succeeded && result.data) {
            Notify.success(t("learning.checkout.success"));
            await enrollmentStore.getMyEnrollments();
            open.value = false;
            router.push({ name: "learning-my-courses" });
          } else {
            Notify.error(t("learning.checkout.error"));
          }
        },
        onCancel: () => {
          // Buyer closed the popup / clicked cancel — not an error, let them retry.
          Notify.info(t("learning.checkout.cancelled"));
        },
        onError: (err: unknown) => {
          console.error("PayPal Buttons error:", err);
          Notify.error(t("learning.checkout.error"));
        },
      })
      .render(buttonsContainer.value);
  } catch (error) {
    console.error("Failed to load/render PayPal Buttons:", error);
    renderError.value = t("learning.checkout.error");
  } finally {
    rendering.value = false;
  }
};

// immediate: true because this component is only ever mounted via v-if once
// its parent already has modelValue=true (see catalog.vue) — the watcher
// would otherwise never see a genuine false-to-true transition to react to.
watch(
  open,
  (isOpen) => {
    if (isOpen) renderButtons();
  },
  { immediate: true },
);
</script>

<template>
  <v-dialog v-model="open" max-width="480">
    <v-card>
      <v-card-title>{{ t("learning.checkout.title") }}</v-card-title>

      <v-card-text>
        <div class="d-flex align-center justify-space-between mb-4">
          <span class="text-body-1">{{ course.title }}</span>
          <span class="text-h6 font-weight-bold">{{ course.amount }} €</span>
        </div>

        <v-alert v-if="renderError" type="error" variant="tonal" class="mb-4">
          {{ renderError }}
        </v-alert>

        <div v-if="rendering && !sdkState.loaded" class="text-center py-6">
          <v-progress-circular indeterminate color="primary" />
          <p class="text-caption text-medium-emphasis mt-2">
            {{ t("learning.checkout.loading") }}
          </p>
        </div>

        <div ref="buttonsContainer" />
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn variant="text" @click="open = false">{{ t("shared.cancel") }}</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
