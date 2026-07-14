<script setup lang="ts">
import { useLessonPageStore } from "@/stores/lessonPage";
import { useSanitizedHtml } from "@/composables/useSanitizedHtml";
import { ErrorType } from "@/primitives/error";
// @ts-ignore - reveal.js type definitions not available
import Reveal from "reveal.js";
import "reveal.js/dist/reveal.css";
import "reveal.js/dist/theme/black.css";

const props = defineProps<{
  lessonId: string;
  // Disable when hosted inside a dialog so reveal.js does not rewrite the page URL hash
  hashNavigation?: boolean;
}>();

const emit = defineEmits<{
  (e: "ready", instance: typeof Reveal): void;
  (e: "forbidden", message: string): void;
}>();

const lessonPageStore = useLessonPageStore();
const { pages, currentPage, loading } = storeToRefs(lessonPageStore);
const { sanitize } = useSanitizedHtml();

const revealRef = ref<HTMLDivElement>();
let revealInstance: typeof Reveal | null = null;

const visitPage = async (slideIndex: number) => {
  const page = pages.value[slideIndex];
  if (!page) return;

  lessonPageStore.lastVisitedPageId = page.id;

  if (!page.completed) {
    await lessonPageStore.markPageCompleted(page.id);
  }
};

onMounted(async () => {
  const result = await lessonPageStore.getLessonPlayer(props.lessonId);

  if (!result.succeeded) {
    if (result.errors?.errorType === ErrorType.Forbidden) {
      emit("forbidden", result.errors.message);
    }
    return;
  }

  await nextTick();

  if (revealRef.value) {
    revealInstance = new Reveal(revealRef.value, {
      hash: props.hashNavigation ?? true,
      transition: "slide",
      width: "100%",
      height: "100%",
      margin: 0.1,
      // ESC is bound to reveal's overview mode by default; free it so the
      // surrounding v-dialog can handle ESC-to-close
      keyboard: { 27: null },
      touch: true,
    });

    await revealInstance.initialize();

    // Resume at the last visited page when available
    const lastVisitedIndex = pages.value.findIndex(
      (p) => p.id === lessonPageStore.lastVisitedPageId,
    );
    if (lastVisitedIndex > 0) {
      revealInstance.slide(lastVisitedIndex, 0);
    }

    revealInstance.addEventListener("slidechanged", async (event: any) => {
      await visitPage(event.indexh || 0);
    });

    // slidechanged does not fire for the initially displayed slide
    const { h } = revealInstance.getIndices();
    await visitPage(h || 0);

    emit("ready", revealInstance);
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
          <div v-else-if="page.contentType === 'Embed' && page.mediaUrl" class="slide-media">
            <iframe
              :src="page.mediaUrl"
              :title="page.title"
              width="80%"
              height="400"
              frameborder="0"
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
              allowfullscreen
            />
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
