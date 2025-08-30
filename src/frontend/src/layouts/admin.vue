<script lang="ts" setup>
import Logo from "@/components/shared/Logo.vue";

import VerticalNavLayout from "./components/admin/vertical-nav/VerticalNavLayout.vue";
import NavItems from "./components/admin/vertical-nav/NavItems.vue";

import NavbarLanguageSwitcher from "./components/admin/navbar/NavbarLanguageSwitcher.vue";
import NavbarThemeSwitcher from "./components/admin/navbar/NavbarThemeSwitcher.vue";
import UserProfile from "./components/admin/navbar/UserProfile.vue";

import FooterLinks from "./components/admin/footer/Footer.vue";
</script>

<template>
  <vertical-nav-layout>
    <template #navbar="{ toggleVerticalOverlayNavActive }">
      <div class="d-flex h-100 align-center">
        <icon-btn
          class="ms-n3 d-lg-none"
          @click="toggleVerticalOverlayNavActive(true)"
        >
          <v-icon icon="bx-menu" />
        </icon-btn>

        <v-spacer />

        <navbar-language-switcher class="me-1" />

        <navbar-theme-switcher class="me-1" />

        <user-profile />
      </div>
    </template>

    <template #vertical-nav-header="{ toggleIsOverlayNavActive }">
      <logo class="app-title-wrapper" />

      <icon-btn
        class="d-block d-lg-none"
        @click="toggleIsOverlayNavActive(false)"
      >
        <v-icon icon="bx-x" />
      </icon-btn>
    </template>

    <template #vertical-nav-content>
      <nav-items />
    </template>

    <RouterView />

    <template #footer>
      <footer-links />
    </template>
  </vertical-nav-layout>
</template>

<style lang="scss">
@use "@layouts/styles/default-layout";
</style>
