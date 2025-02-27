<script setup>
import { storeToRefs } from "pinia";
import { useAuthStore, useAppStore, useTwoFactorAuthStore } from "@/stores";
import TwoFactorEnableDialog from "./TwoFactorEnableDialog.vue";
import TwoFactorDisableDialog from "./TwoFactorDisableDialog.vue";
import TwoFactorStatus from "./TwoFactorStatus.vue";
import { ref } from "vue";

const authStore = useAuthStore();
const appStore = useAppStore();
const twoFactorAuthStore = useTwoFactorAuthStore();
const { user } = storeToRefs(authStore);

const showTwoFactorDialog = ref(false);
const showConfirmDialog = ref(false);

const handleTwoFactorDisabled = async () => {
  appStore.setPageLoading(true);

  try {
    await twoFactorAuthStore.disable();
    showConfirmDialog.value = false;
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoading(false);
  }
};

const handleEnableClick = () => {
  showTwoFactorDialog.value = true;
};

const handleDisableClick = () => {
  showConfirmDialog.value = true;
};
</script>

<template>
  <v-row>
    <two-factor-status :is-two-factor-enabled="user.twoFactorEnabled" @enable="handleEnableClick" @disable="handleDisableClick" />
    <two-factor-enable-dialog v-model="showTwoFactorDialog" />
    <two-factor-disable-dialog v-model="showConfirmDialog" @confirm="handleTwoFactorDisabled" />
  </v-row>
</template>
