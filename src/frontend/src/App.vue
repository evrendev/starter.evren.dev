<script setup lang="ts">
import AppNotification from "@/layouts/components/AppNotification.vue";
import Loader from "@/layouts/components/Loader.vue";
import GoToTop from "@/layouts/components/GoToTop.vue";
</script>

<template>
  <VApp>
    <Loader />
    <RouterView />
    <AppNotification />
    <GoToTop />
  </VApp>
</template>
