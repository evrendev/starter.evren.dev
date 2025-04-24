<script setup>
import { onMounted } from "vue";
import { useCustomizerStore, useNavigationStore } from "@/stores";
import { NavGroup, NavItem, NavCollapse, VersionInfo } from "./";
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
    <div class="pa-5">
      <logo />
    </div>
    <perfect-scrollbar class="scrollnavbar">
      <v-list class="pa-4">
        <template v-for="(item, i) in navigationStore.sidebarItems" :key="i">
          <nav-group :item="item" v-if="item.header" :key="item.title" />
          <v-divider class="my-3" v-else-if="item.divider" />
          <nav-collapse class="leftPadding" :item="item" :level="0" v-else-if="item.children" />
          <nav-item :item="item" v-else class="leftPadding" />
        </template>
      </v-list>
      <version-info v-if="versionInfo" :version="versionInfo?.version" :build-time="versionInfo?.buildTime" class="text-center" />
    </perfect-scrollbar>
  </v-navigation-drawer>
</template>
