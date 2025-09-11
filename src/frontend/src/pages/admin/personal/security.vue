<script lang="ts" setup>
import { Notify } from "@/stores/notification";
import { Security, Tabs } from "@/views/admin/personal";

import { usePersonalStore } from "@/stores/personal";
import { ChangePasswordRequest } from "@/requests/user";
import { DefaultApiResponse } from "@/responses/api";
import {
  DisableTwoFactorAuthenticationRequest,
  EnableTwoFactorAuthenticationRequest,
} from "@/requests/personal";
import { SetupTwoFactorAuthenticationResponse } from "@/responses/personal";
const useProfile = usePersonalStore();
const { user, loading } = storeToRefs(useProfile);

const { t } = useI18n();

const handleChangePassword = async (values: ChangePasswordRequest) => {
  const response: DefaultApiResponse<string> =
    await useProfile.changePassword(values);

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
  const response: DefaultApiResponse<string[]> =
    await useProfile.enableTwoFactorAuthentication(request);

  if (response.succeeded) {
    Notify.success(t("admin.personal.security.notifications.twoFactorEnabled"));
    setupData.value!.showSetup = false;
  } else {
    Notify.error(
      t("admin.personal.security.notifications.twoFactorEnableFailed"),
    );
  }
};

const handleDisableTwoFactorAuthentication = async () => {
  const response: DefaultApiResponse<boolean> =
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

const handleSetupTwoFactorAuthentication = async () => {
  const response: DefaultApiResponse<SetupTwoFactorAuthenticationResponse> =
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
