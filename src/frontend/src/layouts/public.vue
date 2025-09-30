<script setup lang="ts">
import Header from "./components/public/header/Header.vue";
import Footer from "./components/public/footer/Footer.vue";
</script>
<template>
  <div class="page-wrapper">
    <Header />
    <router-view />
    <Footer />
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/public/default";
</style>
