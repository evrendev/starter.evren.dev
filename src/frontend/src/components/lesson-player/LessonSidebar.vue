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
const neutralIcon = computed(() => vuetifyTheme.current.value.colors["grey-400"]);
const sidebarBg = computed(() => vuetifyTheme.current.value.colors.surface);
const sidebarBorder = computed(() => vuetifyTheme.current.value.colors["grey-300"]);
const hoverBg = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "0f");

// All names verified against the generated iconify subset (icons.css)
const contentTypeIcons: Record<string, string> = {
  Text: "bx-align-left",
  Image: "bx-image",
  Video: "bx-play-circle",
  Quiz: "bx-help-circle",
  Embed: "bx-link-alt",
};

type BadgeState = "active" | "completed" | "neutral";

const badgeState = (page: { id: string; completed?: boolean }): BadgeState => {
  if (page.id === lastVisitedPageId.value) return "active";
  return page.completed ? "completed" : "neutral";
};

// Red in dark theme is the secondary token (primary is gray there)
const badgeColor = (state: BadgeState) => {
  if (state === "active") return isDark.value ? "secondary" : "primary";
  if (state === "completed") return "success";
  return undefined;
};

const badgeIcon = (state: BadgeState, contentType: string) =>
  state === "completed"
    ? "bx-bxs-check-circle"
    : (contentTypeIcons[contentType] ?? "bx-align-left");

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
          <v-icon icon="bx-book-open" color="primary" />
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
      <!-- Unset color would default the active-row tint to "primary", which
           is gray in dark theme (see badgeColor for the same reasoning) -->
      <v-list-item
        v-for="(page, index) in pages"
        :key="page.id"
        @click="handlePageClick(index)"
        class="cursor-pointer"
        :active="page.id === lastVisitedPageId"
        :color="isDark ? 'secondary' : 'primary'"
      >
        <template #prepend>
          <v-avatar
            size="30"
            :color="badgeColor(badgeState(page))"
            class="page-badge mr-3"
            :class="{ 'page-badge--neutral': badgeState(page) === 'neutral' }"
          >
            <v-icon :icon="badgeIcon(badgeState(page), page.contentType)" size="18" />
          </v-avatar>
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
  background: v-bind(sidebarBg);
  border-right: 1px solid v-bind(sidebarBorder);
  overflow-y: auto;
}

.cursor-pointer {
  cursor: pointer;
  transition: background-color 0.2s;
}

.cursor-pointer:hover {
  background-color: v-bind(hoverBg);
}

/* background-image paints over the color prop's background-color */
.progress-gradient :deep(.v-progress-linear__determinate) {
  background-image: linear-gradient(
    90deg,
    v-bind(gradientFrom),
    v-bind(gradientTo)
  );
}

.page-badge--neutral {
  border: 1px solid v-bind(neutralIcon);
  color: v-bind(neutralIcon);
}

/* Wrap long page titles onto two lines instead of hard ellipsis */
.lesson-sidebar :deep(.v-list-item-title) {
  white-space: normal;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
