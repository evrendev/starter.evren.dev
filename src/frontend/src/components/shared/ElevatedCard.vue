<script setup lang="ts">
// Shared "white surface + soft shadow (light only)" card shell — introduced
// in Task P1's bugfix for My Courses' stat cards, reused by Course Catalog's
// filter bar (Task P3) instead of re-declaring the same two CSS rules again.
// Same precedent as LessonPlayer.vue's .slide-card--elevated: dark relies on
// the surface/background token contrast, no added shadow needed there.
import { computed } from "vue";
import { useTheme } from "vuetify";

const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
</script>

<template>
  <v-card
    variant="flat"
    class="elevated-card"
    :class="{ 'elevated-card--elevated': !isDark }"
  >
    <slot />
  </v-card>
</template>

<style scoped>
.elevated-card {
  background: rgb(var(--v-theme-surface));
}

.elevated-card--elevated {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}
</style>
