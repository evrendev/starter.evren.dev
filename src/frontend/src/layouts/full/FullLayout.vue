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
        <div>
          <router-view />
        </div>
      </v-container>
      <v-container fluid class="pt-0">
        <div>
          <footer-panel />
        </div>
      </v-container>
    </v-main>
  </v-app>
</template>
