<script setup lang="ts">
// @ts-ignore - reveal.js type definitions not available
import Reveal from "reveal.js";
import { useTheme } from "vuetify";
import { useLessonPageStore } from "@/stores/lessonPage";

const props = defineProps<{
  revealInstance?: typeof Reveal | null;
}>();

const lessonPageStore = useLessonPageStore();
const { pages, progressPercent, currentPage, lastVisitedPageId } =
  storeToRefs(lessonPageStore);

// Light theme: primary→accent gradient fill; dark keeps the flat primary bar
const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
const gradientFrom = computed(() => vuetifyTheme.current.value.colors.primary);
const gradientTo = computed(() => vuetifyTheme.current.value.colors.accent);

const handlePageClick = (index: number) => {
  props.revealInstance?.slide(index, 0);
};
</script>

<template>
  <aside class="lesson-sidebar">
    <v-list class="pa-4">
      <!-- Lesson Title -->
      <v-list-item v-if="currentPage" class="mb-4">
        <template #prepend>
          <v-icon icon="mdi-book-open" color="primary" />
        </template>
        <v-list-item-title class="font-weight-bold">
          {{ currentPage.lessonTitle }}
        </v-list-item-title>
      </v-list-item>

      <!-- Progress Bar -->
      <div class="mb-6">
        <!-- Dark primary is gray; the design reference wants the flat red
             (= dark theme's secondary token) there -->
        <v-progress-linear
          :model-value="progressPercent"
          :color="isDark ? 'secondary' : 'primary'"
          height="8"
          class="mb-2"
          :class="{ 'progress-gradient': !isDark }"
        />
        <div class="text-caption text-center">
          {{ progressPercent }}% Complete
        </div>
      </div>

      <v-divider class="mb-4" />

      <!-- Pages List -->
      <v-list-item
        v-for="(page, index) in pages"
        :key="page.id"
        @click="handlePageClick(index)"
        class="cursor-pointer"
        :active="page.id === lastVisitedPageId"
      >
        <template #prepend>
          <v-icon :icon="page.completed ? 'mdi-check-circle' : 'mdi-circle-outline'" :color="page.completed ? 'success' : 'grey'" size="small" />
        </template>

        <v-list-item-title class="text-body2">
          {{ page.order }}. {{ page.title }}
        </v-list-item-title>

        <v-list-item-subtitle class="text-caption">
          {{ page.contentType }}
        </v-list-item-subtitle>
      </v-list-item>
    </v-list>
  </aside>
</template>

<style scoped>
.lesson-sidebar {
  width: 300px;
  flex-shrink: 0;
  height: 100%;
  background: white;
  border-right: 1px solid #e0e0e0;
  overflow-y: auto;
}

.cursor-pointer {
  cursor: pointer;
  transition: background-color 0.2s;
}

.cursor-pointer:hover {
  background-color: #f5f5f5;
}

/* background-image paints over the color prop's background-color */
.progress-gradient :deep(.v-progress-linear__determinate) {
  background-image: linear-gradient(
    90deg,
    v-bind(gradientFrom),
    v-bind(gradientTo)
  );
}
</style>
