<script setup lang="ts">
import { useLessonPageStore } from "@/stores/lessonPage";
import { useSanitizedHtml } from "@/composables/useSanitizedHtml";
// @ts-ignore - reveal.js type definitions not available
import Reveal from "reveal.js";
import "reveal.js/dist/reveal.css";
import "reveal.js/dist/theme/black.css";

const props = defineProps<{
  lessonId: string;
}>();

const lessonPageStore = useLessonPageStore();
const { pages, currentPage, loading } = storeToRefs(lessonPageStore);
const { sanitize } = useSanitizedHtml();

const revealRef = ref<HTMLDivElement>();
let revealInstance: typeof Reveal | null = null;

onMounted(async () => {
  await lessonPageStore.getLessonPlayer(props.lessonId);

  await nextTick();

  if (revealRef.value) {
    revealInstance = new Reveal(revealRef.value, {
      hash: true,
      transition: "slide",
      width: "100%",
      height: "100%",
      margin: 0.1,
      keyboard: true,
      touch: true,
    });

    await revealInstance.initialize();

    revealInstance.addEventListener("slidechanged", async (event: any) => {
      const currentSlideIndex = event.indexh || 0;
      const pageId = pages.value[currentSlideIndex]?.id;
      if (pageId) {
        await lessonPageStore.markPageCompleted(pageId);
      }
    });
  }
});

onBeforeUnmount(() => {
  if (revealInstance) {
    // Note: Reveal.js 5.x has limited destroy() support; cleanup is incomplete
    // Full memory cleanup may require manual DOM cleanup or future Reveal.js updates
    try {
      revealInstance.destroy?.();
    } catch (e) {
      // Silently handle partial cleanup
    }
  }
});
</script>

<template>
  <div ref="revealRef" class="reveal">
    <div class="slides">
      <template v-if="!loading">
        <section
          v-for="(page, index) in pages"
          :key="page.id"
          class="lesson-slide"
        >
          <h2>{{ page.title }}</h2>
          <div
            class="slide-content"
            :innerHTML="sanitize(page.content)"
          />
          <div v-if="page.contentType === 'Video' && page.mediaUrl" class="slide-media">
            <video width="80%" height="auto" controls>
              <source :src="page.mediaUrl" type="video/mp4" />
              Your browser does not support the video tag.
            </video>
          </div>
          <div v-else-if="page.contentType === 'Image' && page.mediaUrl" class="slide-media">
            <img :src="page.mediaUrl" :alt="page.title" width="80%" />
          </div>
        </section>
      </template>
      <section v-else>
        <v-progress-circular indeterminate color="primary" />
      </section>
    </div>
  </div>
</template>

<style scoped>
.reveal {
  width: 100%;
  height: 100%;
}

.slides {
  width: 100%;
  height: 100%;
}

.lesson-slide {
  text-align: left;
  padding: 40px;
  background: #f5f5f5;
  min-height: 100%;
}

.slide-content {
  font-size: 18px;
  line-height: 1.6;
  color: #333;
  margin: 20px 0;
  word-wrap: break-word;
  overflow-wrap: break-word;
}

.slide-media {
  margin: 20px 0;
  display: flex;
  justify-content: center;
}

.slide-media img,
.slide-media video {
  max-width: 100%;
  height: auto;
  border-radius: 8px;
}
</style>
