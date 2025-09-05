<script setup lang="ts">
import logo from "@images/svg/logo.svg?raw";

const { t } = useI18n();
</script>

<template>
  <router-link to="/" class="app-logo">
    <div class="d-flex" v-html="logo" />
    <h1 class="app-logo-title" v-text="t('app.title')" />
  </router-link>
</template>
