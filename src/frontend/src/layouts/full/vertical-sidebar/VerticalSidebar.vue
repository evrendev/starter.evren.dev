<script setup>
import { onMounted } from "vue";
import { useCustomizerStore, useNavigationStore } from "@/stores";
import { NavGroup, NavItem, NavCollapse } from "./";
import { LogoMain } from "../logo/";

const customizer = useCustomizerStore();
const navigationStore = useNavigationStore();

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
      <logo-main />
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
      <div class="pa-4 text-center">
        <v-chip color="inputBorder" size="small"> v1.0.1 </v-chip>
      </div>
    </perfect-scrollbar>
  </v-navigation-drawer>
</template>
