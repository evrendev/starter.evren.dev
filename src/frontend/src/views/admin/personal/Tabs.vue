<script setup lang="ts">
const { t } = useI18n();
const router = useRoute();
const activeTab = ref(router.name);

const tabs = [
  {
    title: t("admin.personal.profile.title"),
    icon: "bx-user",
    tab: "personal-profile",
  },
  {
    title: t("admin.personal.security.title"),
    icon: "bx-lock-open",
    tab: "personal-security",
  },
  {
    title: t("admin.personal.logs.title"),
    icon: "bx-bell",
    tab: "personal-logs",
  },
];
</script>

<template>
  <v-tabs v-model="activeTab" show-arrows class="v-tabs-pill">
    <v-tab
      v-for="item in tabs"
      size="small"
      :key="item.icon"
      :value="item.tab"
      :to="{ name: item.tab }"
    >
      <v-icon start :icon="item.icon" />
      {{ item.title }}
    </v-tab>
  </v-tabs>
</template>
