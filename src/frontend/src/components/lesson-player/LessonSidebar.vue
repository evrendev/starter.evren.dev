<script setup lang="ts">
// @ts-ignore - reveal.js type definitions not available
import Reveal from "reveal.js";
import { useTheme } from "vuetify";
import { useI18n } from "vue-i18n";
import { useLessonPageStore } from "@/stores/lessonPage";
import { contentTypeIcons } from "@/utils/contentTypeIcons";

const props = defineProps<{
  revealInstance?: typeof Reveal | null;
  categoryTitle?: string | null;
}>();

const { t } = useI18n();
const lessonPageStore = useLessonPageStore();
const { pages, progressPercent, currentPage, lastVisitedPageId } =
  storeToRefs(lessonPageStore);

// Light theme: primary→accent gradient fill; dark keeps the flat primary bar
const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
const gradientFrom = computed(() => vuetifyTheme.current.value.colors.primary);
const gradientTo = computed(() => vuetifyTheme.current.value.colors.accent);
const neutralIcon = computed(() => vuetifyTheme.current.value.colors["grey-400"]);
// Dark's own surface token already separates from the slide's darker
// "background" token elsewhere (see LessonPlayer.vue slideBg); light has no
// such split (slide also uses "surface"), so light is pinned to a literal
// white here instead of re-deriving the same token the surrounding dialog
// chrome also uses, and gets a card-style shadow for separation.
const sidebarBg = computed(() =>
  isDark.value ? vuetifyTheme.current.value.colors.surface : "#FFFFFF",
);
const sidebarBorder = computed(() => vuetifyTheme.current.value.colors["grey-300"]);
const hoverBg = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "0f");
const categoryColor = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "99");
// Same red in both themes (light primary === dark secondary, see LessonSidebar
// active-row / progress-bar reasoning above), resolved to a hex for v-bind CSS
const percentColor = computed(() =>
  isDark.value ? vuetifyTheme.current.value.colors.secondary : vuetifyTheme.current.value.colors.primary,
);
const percentLabelColor = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "99");
// Dark's default on-surface (paletteAntiFlashWhite) already reads correctly
// against the dark sidebar, so it's kept as-is; light's on-surface (#2B2D42)
// reads noticeably lighter than the near-black reference design wants, so
// light is pinned to grey-900 instead of the theme's own on-surface token
const titleColor = computed(() =>
  isDark.value ? vuetifyTheme.current.value.colors["on-surface"] : vuetifyTheme.current.value.colors["grey-900"],
);

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
      <!-- Category label: the enrolled course's Category title (e.g. "Berufliche
           Bildung"), passed down from my-courses.vue via CourseEnrollmentDto.
           Hidden when unavailable (e.g. the standalone /lessons/:id/player route). -->
      <div v-if="categoryTitle" class="category-label mb-1">
        {{ categoryTitle }}
      </div>

      <!-- Lesson Title -->
      <v-list-item v-if="currentPage" class="mb-4 lesson-title-row">
        <v-list-item-title class="lesson-title-text">
          {{ currentPage.lessonTitle }}
        </v-list-item-title>
      </v-list-item>

      <!-- Progress: percent readout first, bar second -->
      <div class="mb-6">
        <div class="percent-row mb-2">
          <span class="percent-value">{{ progressPercent }}%</span>
          <span class="percent-label">{{ t("learning.myCourses.percentComplete") }}</span>
        </div>
        <!-- Dark primary is gray; the design reference wants the flat red
             (= dark theme's secondary token) there -->
        <v-progress-linear
          :model-value="progressPercent"
          :color="isDark ? 'secondary' : 'primary'"
          height="8"
          :class="{ 'progress-gradient': !isDark }"
        />
      </div>

      <v-divider class="mb-4" />

      <!-- Pages List -->
      <!-- Unset color would default the active-row tint to "primary", which
           is gray in dark theme (see badgeColor for the same reasoning) -->
      <v-list-item
        v-for="(page, index) in pages"
        :key="page.id"
        @click="handlePageClick(index)"
        class="cursor-pointer page-list-item"
        :active="page.id === lastVisitedPageId"
        :color="isDark ? 'secondary' : 'primary'"
        rounded="lg"
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

        <v-list-item-title class="text-body2 page-title-text">
          {{ page.order }}. {{ page.title }}
        </v-list-item-title>

        <v-list-item-subtitle class="text-caption page-type-text">
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
  /* Card-style separation from the surrounding dialog chrome, which can
     share the same white/surface tone in light theme */
  box-shadow: 2px 0 12px rgba(0, 0, 0, 0.08);
  overflow-y: auto;
}

.category-label {
  padding: 0 16px;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  color: v-bind(categoryColor);
}

.lesson-title-text {
  font-size: 21px;
  font-weight: 700;
  line-height: 1.25;
  color: v-bind(titleColor);
}

.percent-row {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  width: 100%;
  gap: 8px;
}

.percent-value {
  font-size: 34px;
  font-weight: 800;
  line-height: 1;
  color: v-bind(percentColor);
}

.percent-label {
  font-size: 13px;
  font-weight: 400;
  color: v-bind(percentLabelColor);
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

/* Wrap long page titles instead of a hard ellipsis. 3 lines (not 2) so
   titles like "4. Wissenscheck: Physikalische Grundlagen" fit in full;
   line-clamp only grows rows that actually need it, short titles stay
   single-line, so the general list row height is unaffected. */
.lesson-sidebar :deep(.v-list-item-title) {
  white-space: normal;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Title vs. ContentType-label contrast: medium-weight title, light-weight tag */
.page-list-item :deep(.page-title-text) {
  font-size: 13px;
  font-weight: 500;
}

.page-list-item :deep(.page-type-text) {
  font-weight: 400;
  opacity: 0.8;
}
</style>
