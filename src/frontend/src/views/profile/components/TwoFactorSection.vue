<script setup>
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useAuthStore, useAppStore, useTwoFactorAuthStore } from "@/stores";
import TwoFactorDialog from "./TwoFactorDialog.vue";
import { ref } from "vue";

const { t } = useI18n();
const authStore = useAuthStore();
const appStore = useAppStore();
const twoFactorAuthStore = useTwoFactorAuthStore();
const { user } = storeToRefs(authStore);

const showTwoFactorDialog = ref(false);

const handleTwoFactorDisabled = async () => {
  appStore.setPageLoader(true);

  try {
    twoFactorAuthStore.disable();
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoader(false);
  }
};
</script>

<template>
  <v-row>
    <v-col cols="12">
      <div class="d-flex align-center justify-space-between">
        <div>
          <h3 class="text-h6 mb-2">{{ t("admin.profile.twoFactorAuth.title") }}</h3>
          <p class="text-body-2">{{ t("admin.profile.twoFactorAuth.description") }}</p>
        </div>
        <div>
          <v-btn v-if="!user.twoFactorEnabled" color="primary" prepend-icon="$shieldAccount" @click="showTwoFactorDialog = true">
            {{ t("admin.profile.twoFactorAuth.enable") }}
          </v-btn>
          <v-btn v-else color="error" prepend-icon="$shieldLockOpen" @click="handleTwoFactorDisabled">
            {{ t("admin.profile.twoFactorAuth.disable") }}
          </v-btn>
        </div>
      </div>
    </v-col>

    <two-factor-dialog v-model="showTwoFactorDialog" />
  </v-row>
</template>
