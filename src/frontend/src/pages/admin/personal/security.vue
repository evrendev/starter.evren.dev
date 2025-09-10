<script lang="ts" setup>
import { Notify } from "@/stores/notification";
import { Security, Tabs } from "@/views/admin/personal";

import { usePersonalStore } from "@/stores/personal";
import { ChangePasswordRequest } from "@/requests/user";
import { DefaultApiResponse } from "@/responses/api";
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
</script>

<template>
  <div>
    <tabs />

    <v-window class="mt-5 disable-tab-transition">
      <v-window-item>
        <security
          :loading="loading"
          :two-factor-enabled="user?.twoFactorEnabled ?? false"
          @submit="handleChangePassword"
        />
      </v-window-item>
    </v-window>
  </div>
</template>
