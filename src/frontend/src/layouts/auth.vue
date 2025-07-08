<script setup lang="ts">
import AppNotification from "./components/AppNotification.vue";
</script>

<template>
  <div class="layout-wrapper layout-blank" data-allow-mismatch>
    <RouterView />

    <AppNotification />
  </div>
</template>

<style>
.layout-wrapper.layout-blank {
  flex-direction: column;
}
</style>
