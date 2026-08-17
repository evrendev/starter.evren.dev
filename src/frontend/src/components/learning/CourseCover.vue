<script setup lang="ts">
// Shared between my-courses.vue and catalog.vue (Task P1/P2) — real cover
// image when Course.image is set, otherwise a placeholder block matching
// the design (gray zone + image icon + "Course cover" caption).
defineProps<{
  image?: string | null;
  alt?: string;
}>();

const { t } = useI18n();
const BASE_URL = import.meta.env.VITE_APP_BACKEND_BASE_URL;
</script>

<template>
  <div v-if="image" class="course-cover">
    <img :src="`${BASE_URL}/${image}`" :alt="alt" />
  </div>
  <div v-else class="course-cover course-cover--placeholder">
    <v-icon icon="bx-image" size="28" />
    <span class="text-caption mt-1">{{ t("learning.courseCover") }}</span>
  </div>
</template>

<style scoped>
.course-cover {
  height: 140px;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.course-cover--placeholder {
  flex-direction: column;
  background: rgba(var(--v-theme-on-surface), 0.05);
  color: rgba(var(--v-theme-on-surface), 0.4);
}

.course-cover img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>
