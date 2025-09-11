<script lang="ts" setup>
import { ChangePasswordRequest } from "@/requests/user";
import { ChangePasswordForm, TwoFactorAuthentication } from "./components";
import {
  DisableTwoFactorAuthenticationRequest,
  EnableTwoFactorAuthenticationRequest,
} from "@/requests/personal";
import { SetupTwoFactorAuthenticationResponse } from "@/responses/personal";

defineProps<{
  twoFactorEnabled: boolean;
  setupData: SetupTwoFactorAuthenticationResponse;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: "change-password", values: ChangePasswordRequest): void;
  (
    e: "enable-two-factor-authentication",
    values: EnableTwoFactorAuthenticationRequest,
  ): void;
  (e: "disable-two-factor-authentication"): void;
  (e: "setup-two-factor-authentication"): void;
}>();
</script>

<template>
  <v-row>
    <change-password-form
      :loading="loading"
      @submit="emit('change-password', $event)"
    />

    <two-factor-authentication
      :two-factor-enabled="twoFactorEnabled"
      :setup-data="setupData"
      :loading="loading"
      @enable-two-factor-authentication="
        emit('enable-two-factor-authentication', $event)
      "
      @disable-two-factor-authentication="
        emit('disable-two-factor-authentication')
      "
      @setup-two-factor-authentication="emit('setup-two-factor-authentication')"
    />
  </v-row>
</template>
