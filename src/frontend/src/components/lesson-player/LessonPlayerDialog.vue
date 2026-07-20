<script setup lang="ts">
import { usePageStore } from "@/stores/page";
import { Notify } from "@/stores/notification";
import LessonPlayer from "@/components/lesson-player/LessonPlayer.vue";
import LessonSidebar from "@/components/lesson-player/LessonSidebar.vue";
import NotesPanel from "@/components/lesson-player/NotesPanel.vue";
import LessonProgressSegments from "@/components/lesson-player/LessonProgressSegments.vue";
import LessonMobileToolbar from "@/components/lesson-player/LessonMobileToolbar.vue";
import { useTheme, useDisplay } from "vuetify";
// @ts-ignore - reveal.js type definitions not available
import type Reveal from "reveal.js";

const props = defineProps<{
  modelValue: boolean;
  chapterId: string | null;
  categoryTitle?: string | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const { t } = useI18n();

// v-dialog teleports this template's content out of the component's own
// root DOM node, so Vue's scoped-CSS v-bind() (which sets custom properties
// on that root node) never reaches these elements — verified empirically,
// bindings resolved to transparent/black. Inline :style is used instead,
// since that's set directly on the element and survives the teleport.
const vuetifyTheme = useTheme();
// Modal-wide gray unification (light only): dark already used "surface"
// consistently as its outer-chrome tone across sidebar/content/notes, so
// it's untouched; light previously reused "surface" (pure white) here too,
// which is why the sidebar/content/notes areas all looked like one flat
// white slab instead of a gray backdrop with white cards on top
const containerBg = computed(() =>
  vuetifyTheme.current.value.dark
    ? vuetifyTheme.current.value.colors.surface
    : vuetifyTheme.current.value.colors.background,
);
const closeIconColor = computed(() => vuetifyTheme.current.value.colors["on-surface"]);
const closeHoverBg = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "14");
const contentBorder = computed(() => vuetifyTheme.current.value.colors["grey-300"]);

const pageStore = usePageStore();
const { pages, lastVisitedPageId } = storeToRefs(pageStore);

// No project-wide "mobile" convention found (only VerticalNav's admin-only
// mdAndDown); smAndDown (<960px) is used directly as a sensible default
const { smAndDown: mobile } = useDisplay();
const listSheetOpen = ref(false);
const notesSheetOpen = ref(false);

const revealInstance = ref<typeof Reveal | null>(null);

const currentPageId = computed(() => {
  return lastVisitedPageId.value || pages.value?.[0]?.id || "";
});

const close = () => {
  emit("update:modelValue", false);
};

const handleReady = (instance: typeof Reveal) => {
  revealInstance.value = instance;
};

const handleForbidden = () => {
  Notify.error(t("learning.catalog.notifications.notEnrolled"));
  close();
};
</script>

<template>
  <v-dialog
    :model-value="modelValue"
    class="lesson-player-dialog"
    content-class="lesson-player-dialog-panel"
    transition="lesson-player-dialog-transition"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div
      v-if="chapterId"
      :key="chapterId"
      class="lesson-player-container"
      :class="{ 'lesson-player-container--mobile': mobile }"
      :style="{ backgroundColor: containerBg }"
    >
      <template v-if="mobile">
        <LessonProgressSegments
          :pages="pages"
          :active-page-id="lastVisitedPageId"
          @close="close"
        />
        <div class="mobile-player-content">
          <LessonPlayer
            :chapter-id="chapterId"
            :hash-navigation="false"
            mobile
            @ready="handleReady"
            @forbidden="handleForbidden"
          />
        </div>
        <LessonMobileToolbar
          :reveal-instance="revealInstance"
          @open-list="listSheetOpen = true"
          @open-notes="notesSheetOpen = true"
        />
        <v-bottom-sheet v-model="listSheetOpen" content-class="lesson-mobile-sheet">
          <LessonSidebar
            :reveal-instance="revealInstance"
            :category-title="categoryTitle"
            @page-selected="listSheetOpen = false"
          />
        </v-bottom-sheet>
        <v-bottom-sheet v-model="notesSheetOpen" content-class="lesson-mobile-sheet">
          <NotesPanel v-if="currentPageId" :page-id="currentPageId" />
        </v-bottom-sheet>
      </template>

      <template v-else>
        <button
          class="player-close-btn"
          :style="{ color: closeIconColor, '--close-hover-bg': closeHoverBg }"
          :aria-label="t('shared.close')"
          type="button"
          @click="close"
        >
          <v-icon icon="$close" size="36" />
        </button>

        <LessonSidebar :reveal-instance="revealInstance" :category-title="categoryTitle" />
        <div class="player-main">
          <div class="player-content" :style="{ borderRightColor: contentBorder }">
            <LessonPlayer
              :chapter-id="chapterId"
              :hash-navigation="false"
              @ready="handleReady"
              @forbidden="handleForbidden"
            />
          </div>
          <div class="notes-content" :style="{ backgroundColor: containerBg }">
            <NotesPanel v-if="currentPageId" :page-id="currentPageId" />
          </div>
        </div>
      </template>
    </div>
  </v-dialog>
</template>

<style>
/* Not scoped: the dialog teleports to the overlay container and these
   elements (panel, scrim, transition classes) are rendered by Vuetify. */
.lesson-player-dialog .v-overlay__content.lesson-player-dialog-panel {
  width: 90vw;
  max-width: 1600px;
  /* vh is the mobile-address-bar-hide/show "100vh problem": on real phones
     it's computed against a viewport size that doesn't match what's
     actually visible during the show/hide animation, which is what made
     the mobile toolbar (position: absolute, anchored to this panel's
     height) appear to flicker/disappear. dvh tracks the real visible
     viewport and is the standard fix; vh is kept first as a fallback for
     browsers that don't recognize dvh (they simply ignore that line). */
  height: 90vh;
  height: 90dvh;
  max-height: 90vh;
  max-height: 90dvh;
  margin: 0;
}

.lesson-player-dialog .v-overlay__scrim {
  /* !important: outweigh Vuetify's themed scrim (overlay-scrim-background variable) */
  background: rgba(0, 0, 0, 0.55) !important;
  opacity: 1 !important;
  backdrop-filter: blur(3px);
}

/* Spec: scale(0.95 → 1) + fade, ~260ms ease-out (Vuetify's default
   dialog-transition uses different scale/timing, so a custom one) */
.lesson-player-dialog-transition-enter-active {
  transition:
    opacity 260ms ease-out,
    transform 260ms ease-out;
}

.lesson-player-dialog-transition-leave-active {
  transition:
    opacity 200ms ease-in,
    transform 200ms ease-in;
}

.lesson-player-dialog-transition-enter-from,
.lesson-player-dialog-transition-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

/* Bottom sheets teleport outside this component too; sized so the reused
   LessonSidebar/NotesPanel (which expect a definite-height ancestor) have
   something to resolve their own height:100% against */
.lesson-mobile-sheet {
  max-height: 75vh;
  max-height: 75dvh;
  border-radius: 16px 16px 0 0;
  overflow: hidden;
}

.lesson-mobile-sheet .lesson-sidebar {
  width: 100%;
  height: 75vh;
  height: 75dvh;
  border-right: none;
}

.lesson-mobile-sheet .notes-panel {
  height: 75vh;
  height: 75dvh;
}
</style>

<style scoped>
.lesson-player-container {
  position: relative;
  display: flex;
  height: 100%;
  width: 100%;
  border-radius: 4px;
  overflow: hidden;
}

.lesson-player-container--mobile {
  flex-direction: column;
}

.mobile-player-content {
  flex: 1;
  overflow: auto;
  min-height: 0;
}

.player-close-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  z-index: 10;
  display: flex;
  align-items: center;
  justify-content: center;
  /* 36px icon padded out to a 44x44 hit target */
  width: 44px;
  height: 44px;
  border: none;
  border-radius: 50%;
  background: transparent;
  cursor: pointer;
  transition: background-color 0.2s;
}

.player-close-btn:hover {
  background: var(--close-hover-bg);
}

.player-main {
  flex: 1;
  display: flex;
  flex-direction: row;
  overflow: hidden;
}

.player-content {
  flex: 2;
  overflow: auto;
  border-right: 1px solid;
}

.notes-content {
  flex: 1;
  overflow: hidden;
  padding: 8px;
}
</style>
