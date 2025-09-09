<script setup lang="ts">
const { t } = useI18n();
const router = useRoute();
const activeTab = ref(router.name);

const tabs = [
  {
    title: t("admin.personel.profile.title"),
    icon: "bx-user",
    tab: "personel-profile",
  },
  {
    title: t("admin.personel.security.title"),
    icon: "bx-lock-open",
    tab: "personel-security",
  },
  {
    title: t("admin.personel.logs.title"),
    icon: "bx-bell",
    tab: "personel-logs",
  },
];
</script>

<template>
  <v-tabs v-model="activeTab" show-arrows class="v-tabs-pill">
    <v-tab
      v-for="item in tabs"
      :key="item.icon"
      :value="item.tab"
      :to="{ name: item.tab }"
    >
      <v-icon size="20" start :icon="item.icon" />
      {{ item.title }}
    </v-tab>
  </v-tabs>
</template>
