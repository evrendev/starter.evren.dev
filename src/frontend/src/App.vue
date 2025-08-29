<script setup lang="ts">
import AppNotification from "@/components/shared/AppNotification.vue";
import Loader from "@/components/shared/Loader.vue";
import GoToTop from "@/components/shared/GoToTop.vue";
</script>

<template>
  <VApp>
    <Loader />
    <RouterView />
    <AppNotification />
    <GoToTop />
  </VApp>
</template>
