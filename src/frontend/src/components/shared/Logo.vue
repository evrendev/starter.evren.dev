<script setup lang="ts">
import logo from "@images/svg/logo.svg?raw";
import { useTheme } from "vuetify";
const { name: themeName } = useTheme();

console.log(themeName);
</script>

<template>
  <router-link :to="{ name: 'home' }" class="w-sm-100">
    <div
      :class="{
        'text-primary': themeName === 'light',
        'text-white': themeName === 'dark',
      }"
      class="d-sm-flex align-sm-center justify-sm-center w-sm-100"
      v-html="logo"
    />
  </router-link>
</template>
