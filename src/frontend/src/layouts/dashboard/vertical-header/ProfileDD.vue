<script setup>
import { UserIcon, LogoutIcon } from "vue-tabler-icons";
import { useAuthStore } from "@/stores/auth";
import { useI18n } from "vue-i18n";
const { t } = useI18n();

const authStore = useAuthStore();
const { user } = authStore;
const greeting = () => {
  const time = new Date().getHours();
  if (time < 12) {
    return t("components.verticalHeader.profile.goodMorning");
  } else if (time < 18) {
    return t("components.verticalHeader.profile.goodAfternoon");
  } else {
    return t("components.verticalHeader.profile.goodEvening");
  }
};
</script>

<template>
  <div class="pa-4">
    <h4 class="mb-n1">
      {{ greeting() }},
      <span class="font-weight-regular">
        {{ user.fullName }}
      </span>
    </h4>
    <span class="text-subtitle-2 text-medium-emphasis">
      {{ user.jobtitle }}
    </span>

    <v-divider></v-divider>

    <perfect-scrollbar>
      <v-list class="mt-3">
        <v-list-item color="secondary" rounded="md" value="profile" to="/admin/profile">
          <template v-slot:prepend>
            <user-icon size="20" class="mr-2" />
          </template>

          <v-list-item-title class="text-subtitle-2">
            {{ t("components.verticalHeader.profile.title") }}
          </v-list-item-title>
        </v-list-item>

        <v-list-item @click="authStore.logout()" color="secondary" rounded="md" value="logout">
          <template v-slot:prepend>
            <logout-icon size="20" class="mr-2" />
          </template>

          <v-list-item-title class="text-subtitle-2">
            {{ t("components.verticalHeader.profile.logout") }}
          </v-list-item-title>
        </v-list-item>
      </v-list>
    </perfect-scrollbar>
  </div>
</template>
