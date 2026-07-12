<script setup lang="ts">
import { useLessonPageStore } from "@/stores/lessonPage";
import LessonPlayer from "@/components/lesson-player/LessonPlayer.vue";
import LessonSidebar from "@/components/lesson-player/LessonSidebar.vue";
import NotesPanel from "@/components/lesson-player/NotesPanel.vue";

const route = useRoute();
const lessonId = computed(() => route.params.id as string);

const lessonPageStore = useLessonPageStore();
const { pages } = storeToRefs(lessonPageStore);

const currentPageId = computed(() => {
  return pages.value?.[0]?.id || "";
});

onMounted(async () => {
  if (lessonId.value) {
    await lessonPageStore.getLessonPlayer(lessonId.value);
  }
});

watch(pages, () => {
  // Update current page when pages change
  if (pages.value?.length > 0) {
    // current page updates reactively
  }
});
</script>

<template>
  <div class="lesson-player-container">
    <LessonSidebar />
    <div class="player-main">
      <div class="player-content">
        <LessonPlayer :lesson-id="lessonId" />
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
