<script lang="ts" setup>
import Logo from "@/components/shared/Logo.vue";
import AppNotification from "@/components/shared/AppNotification.vue";
import Loader from "@/components/shared/Loader.vue";
import GoToTop from "@/components/shared/GoToTop.vue";

import {
  //Vertical Navbar
  VerticalNavLayout,
  NavItems,
  //Navbar
  NavbarLanguageSwitcher,
  NavbarThemeSwitcher,
  UserProfile,
  //Footer
  FooterLinks,
} from "./components/admin";
</script>

<template>
  <div>
    <Loader />
    <AppNotification />
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

      <router-view />

      <template #footer>
        <footer-links />
      </template>
    </vertical-nav-layout>
    <GoToTop />
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/admin/default";
</style>
