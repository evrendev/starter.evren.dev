<script setup lang="ts">
import { useLessonPageStore } from "@/stores/lessonPage";
import { Notify } from "@/stores/notification";
import LessonPlayer from "@/components/lesson-player/LessonPlayer.vue";
import LessonSidebar from "@/components/lesson-player/LessonSidebar.vue";
import NotesPanel from "@/components/lesson-player/NotesPanel.vue";
// @ts-ignore - reveal.js type definitions not available
import type Reveal from "reveal.js";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const lessonId = computed(() => route.params.id as string);

const lessonPageStore = useLessonPageStore();
const { pages, lastVisitedPageId } = storeToRefs(lessonPageStore);

const revealInstance = ref<typeof Reveal | null>(null);

const currentPageId = computed(() => {
  return lastVisitedPageId.value || pages.value?.[0]?.id || "";
});

const handleReady = (instance: typeof Reveal) => {
  revealInstance.value = instance;
};

const handleForbidden = async () => {
  Notify.error(t("learning.catalog.notifications.notEnrolled"));
  await router.push({ name: "learning-catalog" });
};
</script>

<template>
  <div class="lesson-player-container">
    <LessonSidebar :reveal-instance="revealInstance" />
    <div class="player-main">
      <div class="player-content">
        <LessonPlayer
          :lesson-id="lessonId"
          @ready="handleReady"
          @forbidden="handleForbidden"
        />
      </div>
      <div class="notes-content">
        <NotesPanel v-if="currentPageId" :lesson-page-id="currentPageId" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.lesson-player-container {
  display: flex;
  height: 100vh;
  width: 100%;
  background: white;
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
