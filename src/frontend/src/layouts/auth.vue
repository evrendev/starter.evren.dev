<script setup lang="ts">
import AppNotification from "./components/AppNotification.vue";
import Loader from "./components/Loader.vue";
</script>

<template>
  <div class="layout-wrapper layout-blank" data-allow-mismatch>
    <Loader />

    <RouterView />

    <AppNotification />
  </div>
</template>

<style>
.layout-wrapper.layout-blank {
  flex-direction: column;
}
</style>
