<script setup>
import { useAuthStore, useCustomizerStore } from "@/stores";
import { SettingsIcon, Menu2Icon, LanguageIcon, SunIcon, MoonIcon } from "vue-tabler-icons";
import { Profile, Languages } from ".";

const customizer = useCustomizerStore();
const authStore = useAuthStore();

const { user } = authStore;
</script>

<template>
  <v-app-bar elevation="0" height="80">
    <v-btn
      class="hidden-md-and-down text-secondary"
      color="lightsecondary"
      icon
      rounded="sm"
      variant="flat"
      @click.stop="customizer.SET_MINI_SIDEBAR(!customizer.miniSidebar)"
      size="small"
    >
      <menu2-icon size="20" stroke-width="1.5" />
    </v-btn>
    <v-btn
      class="hidden-lg-and-up text-secondary ms-3"
      color="lightsecondary"
      icon
      rounded="sm"
      variant="flat"
      @click.stop="customizer.SET_SIDEBAR_DRAWER"
      size="small"
    >
      <menu2-icon size="20" stroke-width="1.5" />
    </v-btn>

    <v-spacer />
    <!-- ---------------------------------------------- -->
    <!-- Notification -->
    <!-- ---------------------------------------------- -->

    <!-- <v-menu :close-on-content-click="false">
      <template v-slot:activator="{ props }">
        <v-btn icon class="text-secondary mx-3" color="lightsecondary" rounded="sm" size="small" variant="flat" v-bind="props">
          <bell-icon stroke-width="1.5" size="22" />
        </v-btn>
      </template>
      <v-sheet rounded="md" width="330" elevation="12">
        <notifications />
      </v-sheet>
    </v-menu> -->

    <!-- ---------------------------------------------- -->
    <!-- Language -->
    <!-- ---------------------------------------------- -->
    <v-menu :close-on-content-click="false">
      <template v-slot:activator="{ props }">
        <v-btn icon class="text-secondary mx-3" color="surface-light" rounded="sm" size="small" variant="flat" v-bind="props">
          <language-icon stroke-width="1.5" size="22" />
        </v-btn>
      </template>
      <v-sheet rounded="md" width="150" elevation="12">
        <languages />
      </v-sheet>
    </v-menu>

    <!-- ---------------------------------------------- -->
    <!-- Theme Toggle -->
    <!-- ---------------------------------------------- -->
    <v-btn
      icon
      class="text-secondary mx-3"
      color="lightsecondary"
      rounded="sm"
      size="small"
      variant="flat"
      @click="customizer.TOGGLE_THEME()"
    >
      <sun-icon v-if="customizer.theme === 'dark'" stroke-width="1.5" size="22" />
      <moon-icon v-else stroke-width="1.5" size="22" />
    </v-btn>

    <!-- ---------------------------------------------- -->
    <!-- User Profile -->
    <!-- ---------------------------------------------- -->
    <v-menu :close-on-content-click="false">
      <template v-slot:activator="{ props }">
        <v-btn class="profileBtn text-primary" color="lightprimary" variant="flat" rounded="pill" v-bind="props">
          <v-avatar class="mr-4" color="primary" size="32">
            <span class="text-h5">{{ user.initial }}</span>
          </v-avatar>
          <settings-icon stroke-width="1.5" />
        </v-btn>
      </template>
      <v-sheet rounded="md" width="300" elevation="12">
        <profile />
      </v-sheet>
    </v-menu>
  </v-app-bar>
</template>
