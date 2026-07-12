<script setup lang="ts">
import { useLessonPageStore } from "@/stores/lessonPage";
import LessonPlayer from "@/components/lesson-player/LessonPlayer.vue";
import LessonSidebar from "@/components/lesson-player/LessonSidebar.vue";

const route = useRoute();
const lessonId = computed(() => route.params.id as string);

const lessonPageStore = useLessonPageStore();

onMounted(async () => {
  if (lessonId.value) {
    await lessonPageStore.getLessonPlayer(lessonId.value);
  }
});
</script>

<template>
  <div class="lesson-player-container">
    <LessonSidebar />
    <div class="player-content">
      <LessonPlayer :lesson-id="lessonId" />
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

.player-content {
  flex: 1;
  overflow: auto;
}
</style>
