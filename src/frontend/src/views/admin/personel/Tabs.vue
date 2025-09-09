<script setup lang="ts">
const router = useRoute();
const activeTab = ref(router.name);

const tabs = [
  { title: "Account", icon: "bx-user", tab: "personel-profile" },
  { title: "Security", icon: "bx-lock-open", tab: "personel-security" },
  { title: "Notifications", icon: "bx-bell", tab: "personel-logs" },
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
