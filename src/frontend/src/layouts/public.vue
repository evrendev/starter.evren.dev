<script setup lang="ts">
import Header from "./components/public/header/Header.vue";
</script>
<template>
  <div class="page-wrapper">
    <Header />
    <div>
      <router-view />
    </div>
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/public/default";
</style>
