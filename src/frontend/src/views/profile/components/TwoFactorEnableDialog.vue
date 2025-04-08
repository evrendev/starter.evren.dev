<script setup>
import { ref, onMounted, watch, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { storeToRefs } from "pinia";
import { useTwoFactorAuthStore } from "@/stores";
import { useForm } from "vee-validate";
import { object, string } from "yup";
import QRCode from "qrcode";

const { t } = useI18n();

const props = defineProps({
  modelValue: {
    type: Boolean,
    required: true
  }
});

const emit = defineEmits(["update:modelValue", "enabled", "disabled"]);

const twoFactorAuthStore = useTwoFactorAuthStore();
const { loading, setupData } = storeToRefs(twoFactorAuthStore);

const qrCodeImage = ref(null);

const schema = object().shape({
  code: string()
    .required(t("admin.profile.validation.twoFactorAuth.code.required"))
    .matches(/^[0-9]{6}$/, t("admin.profile.validation.twoFactorAuth.code.invalid"))
});

const defaultValues = {
  code: ""
};

const { defineField, handleSubmit, resetForm } = useForm({
  validationSchema: schema,
  initialValues: defaultValues
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const [code, codeProps] = defineField("code", vuetifyConfig);

watch(
  () => props.modelValue,
  (newValue) => {
    if (dialog.value !== newValue) {
      dialog.value = newValue;
      if (newValue) {
        setupTwoFactor();
      } else {
        resetDialog();
      }
    }
  }
);

const updateModelValue = (newValue) => {
  emit("update:modelValue", newValue);
};

const dialog = ref(props.modelValue);
watchEffect(() => {
  updateModelValue(dialog.value);
});

const onSubmit = handleSubmit(async (values) => {
  try {
    await twoFactorAuthStore.enable(values.code);
    closeDialog();
  } catch (error) {
    console.error(error);
  }
});

const closeDialog = () => {
  dialog.value = false;
};

const resetDialog = () => {
  code.value = "";
  resetForm();
  qrCodeImage.value = null;
  twoFactorAuthStore.$reset();
};

const setupTwoFactor = async () => {
  try {
    await twoFactorAuthStore.setup();
    if (setupData.value?.qrCodeUri) {
      qrCodeImage.value = await QRCode.toDataURL(setupData.value.qrCodeUri, {
        width: 160,
        margin: 2,
        color: {
          dark: "#000",
          light: "#FFF"
        }
      });
    }
  } catch (error) {
    console.error("Setup error:", error);
    closeDialog();
  }
};

onMounted(() => {
  if (props.modelValue) {
    setupTwoFactor();
  }
});
</script>

<template>
  <v-dialog v-model="dialog" max-width="500" persistent class="dialog-colored-title">
    <v-card>
      <v-form @submit.prevent="onSubmit">
        <v-card-title class="text-h5">
          {{ t("admin.profile.twoFactorAuth.setup.title") }}
        </v-card-title>

        <v-card-text>
          <v-container>
            <v-row class="text-center">
              <v-col>
                <template v-if="loading">
                  <v-progress-circular indeterminate color="primary" />
                </template>

                <template v-else>
                  <v-row>
                    <v-col cols="12" v-if="qrCodeImage">
                      <img :src="qrCodeImage" alt="QR Code" width="160" height="160" class="border rounded" />
                    </v-col>
                  </v-row>

                  <v-row>
                    <v-col cols="12" v-if="setupData.sharedKey">
                      <p class="text-body-2">
                        {{ t("admin.profile.twoFactorAuth.setup.manual") }}
                      </p>
                      <code class="text-body-2 pa-2 bg-grey-lighten-4 rounded d-block">{{ setupData.sharedKey }}</code>
                    </v-col>
                  </v-row>

                  <v-row>
                    <v-col cols="12">
                      <v-text-field
                        v-model="code"
                        v-bind="codeProps"
                        :label="t('admin.profile.twoFactorAuth.setup.code')"
                        :loading="loading"
                        maxlength="6"
                        variant="outlined"
                        density="compact"
                        hide-details="auto"
                      />
                    </v-col>
                  </v-row>
                </template>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>

        <v-card-actions>
          <v-btn color="error" @click="closeDialog" :disabled="loading" class="mr-2">
            {{ t("common.cancel") }}
          </v-btn>
          <v-btn color="primary" type="submit" :loading="loading">
            {{ t("common.verify") }}
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-dialog>
</template>
