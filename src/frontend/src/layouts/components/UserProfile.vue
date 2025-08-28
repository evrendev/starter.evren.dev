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
  <VBadge
    dot
    location="bottom right"
    offset-x="3"
    offset-y="3"
    color="success"
    bordered
  >
    <VAvatar class="cursor-pointer" color="primary" variant="tonal">
      {{ user?.initial }}
      <VMenu activator="parent" width="230" location="bottom end" offset="14px">
        <VList>
          <VListItem>
            <template #prepend>
              <VListItemAction start>
                <VBadge
                  dot
                  location="bottom right"
                  offset-x="3"
                  offset-y="3"
                  color="success"
                >
                  <VAvatar color="primary" variant="tonal">
                    {{ user?.initial }}
                  </VAvatar>
                </VBadge>
              </VListItemAction>
            </template>

            <VListItemTitle class="font-weight-semibold">
              {{ user?.fullName }}
            </VListItemTitle>
            <VListItemSubtitle>
              {{ user?.email }}
            </VListItemSubtitle>
          </VListItem>
          <VDivider class="my-2" />

          <VListItem to="/admin/profile">
            <template #prepend>
              <VIcon class="me-2" icon="bx-user" size="22" />
            </template>

            <VListItemTitle>
              {{ t("admin.components.navbar.profile.title") }}
            </VListItemTitle>
          </VListItem>

          <VListItem @click="logout">
            <template #prepend>
              <VIcon class="me-2" icon="bx-log-out" size="22" />
            </template>

            <VListItemTitle>
              {{ t("admin.components.navbar.profile.logout") }}
            </VListItemTitle>
          </VListItem>
        </VList>
      </VMenu>
    </VAvatar>
  </VBadge>
</template>
