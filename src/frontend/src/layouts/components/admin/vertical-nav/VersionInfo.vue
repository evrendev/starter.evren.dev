<script setup lang="ts">
const { version } = useVersion();
</script>

<template>
  <div
    class="version-info text-center text-subtitle-2 font-weight-light d-flex ga-2 justify-center align-center mb-1"
  >
    <v-icon icon="bx-info-circle" size="16" />
    <span class="font-weight-medium"> v: {{ version }} </span>
  </div>
</template>
