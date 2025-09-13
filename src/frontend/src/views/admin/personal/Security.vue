<script lang="ts" setup>
import { ChangePasswordRequest } from "@/types/requests/user";
import { ChangePasswordForm, TwoFactorAuthentication } from "./components";
import { EnableTwoFactorAuthenticationRequest } from "@/types/requests/personal";
import {
  RecoverCodesResponse,
  SetupTwoFactorAuthenticationResponse,
} from "@/types/responses/personal";

defineProps<{
  twoFactorEnabled: boolean;
  setupData: SetupTwoFactorAuthenticationResponse;
  recoverData: RecoverCodesResponse;
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
      :recover-data="recoverData"
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
