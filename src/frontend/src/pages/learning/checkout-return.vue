<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";

// Fallback-only page (see PayPalService.CreateOrderAsync / Task Q3): the
// primary checkout flow is the PayPal JS SDK Buttons popup in
// CheckoutDialog.vue, which never navigates away from the catalog. PayPal
// only sends the buyer here if that popup was blocked or the SDK fell back
// to a full-page redirect — same query-param-driven "resolve on mount"
// pattern as auth/reset-password.vue and auth/confirm-email.vue.
//
// This page cannot itself call the capture endpoint: PayPal's redirect only
// carries its own order id ("token") and a PayerID, not our internal
// PaymentOrder.Id that POST /payments/orders/{id}/capture requires, and no
// lookup-by-PayPalOrderId endpoint exists (the primary popup flow never
// needs one, so one wasn't built here). If the buyer actually approved the
// payment (PayerID present), Task Q1's webhook job — once a WebhookId is
// configured — will still pick up PAYMENT.CAPTURE.COMPLETED and finish the
// enrollment on its own; this page can only tell the buyer that and point
// them at My Courses / back to the catalog to retry.
const route = useRoute();
const router = useRouter();

const { t } = useI18n();

type Status = "approved" | "cancelled";
const status = ref<Status>("cancelled");

onMounted(() => {
  status.value = route.query.PayerID ? "approved" : "cancelled";
});

const goToMyCourses = () => {
  router.push({ name: "learning-my-courses" });
};

const goToCatalog = () => {
  router.push({ name: "learning-catalog" });
};
</script>

<template>
  <v-container class="py-16 text-center">
    <v-card max-width="480" class="mx-auto pa-6">
      <div v-if="status === 'approved'">
        <v-alert type="info" density="compact" class="mb-4">
          {{ t("learning.checkoutReturn.success") }}
        </v-alert>
        <v-btn color="primary" variant="flat" block @click="goToMyCourses">
          {{ t("learning.checkoutReturn.goToMyCourses") }}
        </v-btn>
      </div>

      <div v-else>
        <v-alert type="warning" density="compact" class="mb-4">
          {{ t("learning.checkoutReturn.error") }}
        </v-alert>
        <v-btn color="primary" variant="flat" block @click="goToCatalog">
          {{ t("learning.checkoutReturn.backToCatalog") }}
        </v-btn>
      </div>
    </v-card>
  </v-container>
</template>
