<script setup lang="ts">
const { t } = useI18n();
const props = defineProps<{
  twoFactorEnabled: boolean;
  loading: boolean;
}>();
const emit = defineEmits<{
  (e: "toggle-two-steps-verification"): void;
}>();

function toggleTwoStepsVerification() {
  emit("toggle-two-steps-verification");
}
</script>
<template>
  <v-col cols="12">
    <v-card :title="t('admin.personal.security.twoStepVerification.title')">
      <v-card-text>
        <p
          class="font-weight-semibold"
          v-text="
            t(
              `admin.personal.security.twoStepVerification.${twoFactorEnabled ? 'enabled' : 'notEnabled'}`,
            )
          "
        />
        <p
          v-text="t('admin.personal.security.twoStepVerification.description')"
        />

        <v-btn
          :color="twoFactorEnabled ? 'danger' : 'primary'"
          variant="elevated"
          :prepend-icon="twoFactorEnabled ? 'bx-lock-open-alt' : 'bx-lock-alt'"
          @click="toggleTwoStepsVerification"
          :loading="loading"
        >
          {{
            twoFactorEnabled
              ? t("admin.personal.security.twoStepVerification.disable")
              : t("admin.personal.security.twoStepVerification.enable")
          }}
        </v-btn>
      </v-card-text>
    </v-card>
  </v-col>
</template>
