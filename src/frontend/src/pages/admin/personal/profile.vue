<script lang="ts" setup>
import { BasicUser } from "@/models/user";
import { Notify } from "@/stores/notification";
import { usePersonalStore } from "@/stores/personal";
import { Profile, Tabs } from "@/views/admin/personal";

const { t } = useI18n();

const useProfile = usePersonalStore();
const { user, loading } = storeToRefs(useProfile);

onMounted(async () => {
  await useProfile.getUser();
});

const submit = async (values: BasicUser) => {
  const response = await useProfile.update(values);

  if (response.succeeded) {
    Notify.success(t("admin.personal.profile.notifications.updated"));
  } else {
    Notify.error(t("admin.personal.profile.notifications.updateFailed"));
  }
};
</script>

<template>
  <div>
    <tabs />

    <v-window class="mt-5 disable-tab-transition">
      <v-window-item>
        <profile :loading="loading" :user="user" @submit="submit" />
      </v-window-item>
    </v-window>
  </div>
</template>
