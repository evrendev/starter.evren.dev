<script setup>
defineProps({
  version: {
    type: String,
    default: null,
    required: false
  },
  buildTime: {
    type: String,
    default: null,
    required: false
  }
});
</script>

<template>
  <div class="d-flex flex-column ga-1 bg-lighten-5 text-muted version-info">
    <div class="d-flex align-center justify-center ga-1">
      <v-icon size="12" color="teal-darken-3" icon="$info" />
      <span>v{{ version }}</span>
    </div>
    <div class="d-flex align-center justify-center ga-1">
      <v-icon size="12" color="teal-darken-3" icon="$clockTimeEightOutline" />
      <span>{{ buildTime }}</span>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.version-info {
  font-size: 0.5rem;
}
</style>
