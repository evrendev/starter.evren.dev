<script setup lang="ts">
import { useAuthStore } from "@/stores/auth";
import { Notify } from "@/stores/notification";
const { t } = useI18n();

const authStore = useAuthStore();
const { user } = storeToRefs(authStore);
const router = useRouter();

const logout = async () => {
  const result = await authStore.logout();
  if (result.succeeded) {
    router.replace("/auth/login");
    Notify.success(result.data ?? t("admin.messages.success.loggedOut"));
  } else {
    Notify.error(result.data ?? t("admin.messages.errors.unknown"));
  }
};
</script>

<template>
  <v-badge
    dot
    location="bottom right"
    offset-x="3"
    offset-y="3"
    color="success"
    bordered
  >
    <v-avatar class="cursor-pointer" color="primary" variant="tonal">
      {{ user?.initial }}
      <v-menu
        activator="parent"
        width="230"
        location="bottom end"
        offset="14px"
      >
        <v-list>
          <v-list-item>
            <template #prepend>
              <v-list-item-action start>
                <v-badge
                  dot
                  location="bottom right"
                  offset-x="3"
                  offset-y="3"
                  color="success"
                >
                  <v-avatar color="primary" variant="tonal">
                    {{ user?.initial }}
                  </v-avatar>
                </v-badge>
              </v-list-item-action>
            </template>

            <v-list-item-title class="font-weight-semibold">
              {{ user?.fullName }}
            </v-list-item-title>
            <v-list-item-subtitle>
              {{ user?.email }}
            </v-list-item-subtitle>
          </v-list-item>
          <v-divider class="my-2" />

          <v-list-item to="/admin/profile">
            <template #prepend>
              <v-icon class="me-2" icon="bx-user" size="22" />
            </template>

            <v-list-item-title>
              {{ t("admin.components.navbar.profile.title") }}
            </v-list-item-title>
          </v-list-item>

          <v-list-item @click="logout">
            <template #prepend>
              <v-icon class="me-2" icon="bx-log-out" size="22" />
            </template>

            <v-list-item-title>
              {{ t("admin.components.navbar.profile.logout") }}
            </v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>
    </v-avatar>
  </v-badge>
</template>
