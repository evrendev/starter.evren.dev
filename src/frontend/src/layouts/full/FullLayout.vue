<script setup>
import { RouterView } from "vue-router";
import { VerticalSidebar } from "./vertical-sidebar";
import { VerticalHeader } from "./vertical-header";
import { useCustomizerStore } from "@/stores";
import FooterPanel from "./footer/FooterPanel.vue";

const customizer = useCustomizerStore();
</script>

<template>
  <v-app
    :theme="customizer.theme"
    :class="[customizer.fontTheme, customizer.miniSidebar ? 'mini-sidebar' : '', customizer.inputBg ? 'inputWithbg' : '']"
  >
    <vertical-sidebar />
    <vertical-header />

    <v-main>
      <v-container fluid class="page-wrapper">
        <router-view />
      </v-container>
      <v-container fluid class="pt-0">
        <footer-panel />
      </v-container>
    </v-main>
  </v-app>
</template>

<style lang="scss" scoped>
:deep(.grecaptcha-logo) {
  display: none !important;
}
</style>
