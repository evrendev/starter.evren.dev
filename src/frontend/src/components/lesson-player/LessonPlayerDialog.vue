<script setup lang="ts">
import { useLessonPageStore } from "@/stores/lessonPage";
import { Notify } from "@/stores/notification";
import LessonPlayer from "@/components/lesson-player/LessonPlayer.vue";
import LessonSidebar from "@/components/lesson-player/LessonSidebar.vue";
import NotesPanel from "@/components/lesson-player/NotesPanel.vue";
// @ts-ignore - reveal.js type definitions not available
import type Reveal from "reveal.js";

const props = defineProps<{
  modelValue: boolean;
  lessonId: string | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const { t } = useI18n();

const lessonPageStore = useLessonPageStore();
const { pages, lastVisitedPageId } = storeToRefs(lessonPageStore);

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
    <div v-if="lessonId" :key="lessonId" class="lesson-player-container">
      <button
        class="player-close-btn"
        :aria-label="t('shared.close')"
        type="button"
        @click="close"
      >
        <v-icon icon="$close" size="36" />
      </button>

      <LessonSidebar :reveal-instance="revealInstance" />
      <div class="player-main">
        <div class="player-content">
          <LessonPlayer
            :lesson-id="lessonId"
            :hash-navigation="false"
            @ready="handleReady"
            @forbidden="handleForbidden"
          />
        </div>
        <div class="notes-content">
          <NotesPanel v-if="currentPageId" :lesson-page-id="currentPageId" />
        </div>
      </div>
    </div>
  </v-dialog>
</template>

<style>
/* Not scoped: the dialog teleports to the overlay container and these
   elements (panel, scrim, transition classes) are rendered by Vuetify. */
.lesson-player-dialog .v-overlay__content.lesson-player-dialog-panel {
  width: 90vw;
  max-width: 1600px;
  height: 90vh;
  max-height: 90vh;
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
</style>

<style scoped>
.lesson-player-container {
  position: relative;
  display: flex;
  height: 100%;
  width: 100%;
  background: white;
  border-radius: 4px;
  overflow: hidden;
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
  color: #2b2d42;
  cursor: pointer;
  transition: background-color 0.2s;
}

.player-close-btn:hover {
  background: rgba(43, 45, 66, 0.08);
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
  border-right: 1px solid #e0e0e0;
}

.notes-content {
  flex: 1;
  overflow: hidden;
  background: white;
  padding: 8px;
}
</style>
