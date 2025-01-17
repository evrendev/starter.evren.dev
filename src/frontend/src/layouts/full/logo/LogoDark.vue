<template>
  <div class="logo">
    <RouterLink to="/">
      <UiLogo />
    </RouterLink>
  </div>
</template>
<script setup lang="ts">
import { RouterLink } from 'vue-router';
import UiLogo from '@/components/shared/UiLogo.vue';
</script>
