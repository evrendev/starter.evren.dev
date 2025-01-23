<script setup>
import { RouterView } from "vue-router";
import VerticalSidebarVue from "./vertical-sidebar/VerticalSidebar.vue";
import VerticalHeaderVue from "./vertical-header/VerticalHeader.vue";
import FooterPanel from "./footer/FooterPanel.vue";
import { useCustomizerStore } from "../../stores/customizer";
const customizer = useCustomizerStore();
</script>

<template>
  <v-app
    :theme="customizer.theme"
    :class="[customizer.fontTheme, customizer.miniSidebar ? 'mini-sidebar' : '', customizer.inputBg ? 'inputWithbg' : '']"
  >
    <VerticalSidebarVue />
    <VerticalHeaderVue />

    <v-main>
      <v-container fluid class="page-wrapper">
        <div>
          <RouterView />
        </div>
      </v-container>
      <v-container fluid class="pt-0">
        <div>
          <FooterPanel />
        </div>
      </v-container>
    </v-main>
  </v-app>
</template>

<style lang="scss">
.grecaptcha-badge {
  display: none !important;
}
</style>
