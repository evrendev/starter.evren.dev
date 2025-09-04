<script setup lang="ts">
defineProps<{
  isActive?: boolean;
}>();
</script>

<template>
  <v-icon v-if="isActive" icon="bx-check" color="success" />
  <v-icon v-else icon="bx-x" color="error" />
</template>
