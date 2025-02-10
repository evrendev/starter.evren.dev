<script setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps({
  isTwoFactorEnabled: {
    type: Boolean,
    required: true
  }
});

const emit = defineEmits(["enable", "disable"]);

const handleEnable = () => {
  emit("enable");
};

const handleDisable = () => {
  emit("disable");
};
</script>

<template>
  <v-col cols="12">
    <div class="d-flex align-center justify-space-between">
      <div>
        <h3 class="text-h6 mb-2">{{ t("admin.profile.twoFactorAuth.title") }}</h3>
        <p class="text-body-2">{{ t("admin.profile.twoFactorAuth.description") }}</p>
      </div>
      <div>
        <v-btn v-if="!isTwoFactorEnabled" color="primary" prepend-icon="$shieldAccount" @click="handleEnable">
          {{ t("admin.profile.twoFactorAuth.enable") }}
        </v-btn>
        <v-btn v-else color="error" prepend-icon="$shieldLockOpen" @click="handleDisable">
          {{ t("admin.profile.twoFactorAuth.disable") }}
        </v-btn>
      </div>
    </div>
  </v-col>
</template>
