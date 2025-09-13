<script lang="ts" setup>
import { Notify } from "@/stores/notification";
import { Security, Tabs } from "@/views/admin/personal";

import { usePersonalStore } from "@/stores/personal";
import { ChangePasswordRequest } from "@/types/requests/user";
import { EnableTwoFactorAuthenticationRequest } from "@/types/requests/personal";
import {
  RecoverCodesResponse,
  SetupTwoFactorAuthenticationResponse,
} from "@/types/responses/personal";
import { Result } from "@/primitives/result";
const useProfile = usePersonalStore();
const { user, loading } = storeToRefs(useProfile);

const { t } = useI18n();

const handleChangePassword = async (values: ChangePasswordRequest) => {
  const response: Result<string> = await useProfile.changePassword(values);

  if (response.succeeded) {
    Notify.success(t("admin.personal.security.notifications.passwordChanged"));
  } else {
    Notify.error(
      t("admin.personal.security.notifications.passwordChangeFailed"),
    );
  }
};

const handleEnableTwoFactorAuthentication = async (
  request: EnableTwoFactorAuthenticationRequest,
) => {
  const response: Result<string[]> =
    await useProfile.enableTwoFactorAuthentication(request);

  if (response.succeeded) {
    Notify.success(t("admin.personal.security.notifications.twoFactorEnabled"));
    setupData.value!.showSetup = false;
    recoverData.value.showRecoverCodes = true;
    recoverData.value.data = response.data || [];
  } else {
    Notify.error(
      t("admin.personal.security.notifications.twoFactorEnableFailed"),
    );
  }
};

const handleDisableTwoFactorAuthentication = async () => {
  const response: Result<boolean> =
    await useProfile.disableTwoFactorAuthentication();

  if (response.succeeded) {
    Notify.success(
      t("admin.personal.security.notifications.twoFactorDisabled"),
    );
  } else {
    Notify.error(
      t("admin.personal.security.notifications.twoFactorDisableFailed"),
    );
  }
};

const setupData = ref<SetupTwoFactorAuthenticationResponse>({
  showSetup: false,
  sharedKey: "",
  qrCodeUri: "",
});

const recoverData = ref<RecoverCodesResponse>({
  data: [],
  showRecoverCodes: false,
});

const handleSetupTwoFactorAuthentication = async () => {
  const response: Result<SetupTwoFactorAuthenticationResponse> =
    await useProfile.setupTwoFactorAuthentication();

  if (response.succeeded && response.data) {
    setupData.value = response.data;
    setupData.value!.showSetup = true;
  } else {
    setupData.value = {
      showSetup: false,
      sharedKey: "",
      qrCodeUri: "",
    };
    Notify.error(
      t("admin.personal.security.notifications.twoFactorSetupFailed"),
    );
  }
};
</script>

<template>
  <div>
    <tabs />

    <v-window class="mt-5 disable-tab-transition">
      <v-window-item>
        <security
          :loading="loading"
          :two-factor-enabled="user?.twoFactorEnabled ?? false"
          :setup-data="setupData!"
          :recover-data="recoverData!"
          @change-password="handleChangePassword"
          @enable-two-factor-authentication="
            handleEnableTwoFactorAuthentication
          "
          @disable-two-factor-authentication="
            handleDisableTwoFactorAuthentication
          "
          @setup-two-factor-authentication="handleSetupTwoFactorAuthentication"
        />
      </v-window-item>
    </v-window>
  </div>
</template>
