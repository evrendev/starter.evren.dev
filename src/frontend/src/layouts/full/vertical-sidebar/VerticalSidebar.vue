<script setup>
import { onMounted } from "vue";
import { useCustomizerStore, useNavigationStore } from "@/stores";
import { NavGroup, NavItem, NavCollapse } from "./";
import { Logo } from "../logo/";

const customizer = useCustomizerStore();
const navigationStore = useNavigationStore();

defineProps({
  versionInfo: {
    required: false
  }
});

onMounted(() => {
  navigationStore.generateSidebarItems();
});
</script>

<template>
  <v-navigation-drawer
    left
    v-model="customizer.sidebarDrawer"
    elevation="0"
    rail-width="75"
    mobile-breakpoint="lg"
    app
    class="leftSidebar"
    :rail="customizer.miniSidebar"
    expand-on-hover
  >
    <!---Logo part -->
    <div class="pa-5">
      <logo />
    </div>
    <!-- ---------------------------------------------- -->
    <!---Navigation -->
    <!-- ---------------------------------------------- -->
    <perfect-scrollbar class="scrollnavbar">
      <v-list class="pa-4">
        <!---Menu Loop -->
        <template v-for="(item, i) in navigationStore.sidebarItems" :key="i">
          <!---Item Sub Header -->
          <nav-group :item="item" v-if="item.header" :key="item.title" />
          <!---Item Divider -->
          <v-divider class="my-3" v-else-if="item.divider" />
          <!---If Has Child -->
          <nav-collapse class="leftPadding" :item="item" :level="0" v-else-if="item.children" />
          <!---Single Item-->
          <nav-item :item="item" v-else class="leftPadding" />
          <!---End Single Item-->
        </template>
      </v-list>
      <div class="d-flex flex-column ga-1 bg-lighten-5 text-muted version-info">
        <div class="d-flex align-center justify-center ga-1">
          <v-icon size="12" color="teal-darken-3" icon="$info" />
          <span>v{{ versionInfo?.version }}</span>
        </div>
        <div class="d-flex align-center justify-center ga-1">
          <v-icon size="12" color="teal-darken-3" icon="$clockTimeEightOutline" />
          <span>{{ versionInfo?.buildTime }}</span>
        </div>
      </div>
    </perfect-scrollbar>
  </v-navigation-drawer>
</template>

<style lang="scss" scoped>
.version-info {
  font-size: 0.5rem;
}
</style>
