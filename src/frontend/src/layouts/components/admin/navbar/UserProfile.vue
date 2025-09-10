<script setup lang="ts">
import { usePersonalStore } from "@/stores/personal";
import { useAuthStore } from "@/stores/auth";
import { useAppStore } from "@/stores/app";
import { Notify } from "@/stores/notification";
const { t } = useI18n();

const authStore = useAuthStore();
const personalStore = usePersonalStore();
const appStore = useAppStore();
const { user } = storeToRefs(personalStore);
const router = useRouter();

if (!user.value) personalStore.getUser();

const logout = async () => {
  appStore.setLoading(true);
  const result = await authStore.logout();
  if (result.succeeded) {
    router.replace("/auth/login");
    Notify.success(result.data ?? t("admin.messages.success.loggedOut"));
    appStore.setLoading(false);
  } else {
    Notify.error(result.data ?? t("admin.messages.errors.unknown"));
    appStore.setLoading(false);
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

          <v-list-item :to="{ name: 'personal-profile' }">
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
