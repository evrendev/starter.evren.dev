<script lang="ts" setup>
import { ChangePasswordRequest } from "@/requests/user";
import { ChangePasswordForm, TwoStepsVerification } from "./components";

defineProps<{
  twoFactorEnabled: boolean;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: "submit", values: ChangePasswordRequest): void;
}>();
</script>

<template>
  <v-row>
    <change-password-form :loading="loading" @submit="emit('submit', $event)" />

    <two-steps-verification />
  </v-row>
</template>
