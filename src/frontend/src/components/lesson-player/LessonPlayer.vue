<script setup lang="ts">
import { useTheme } from "vuetify";
import { useLessonPageStore } from "@/stores/lessonPage";
import { useSanitizedHtml } from "@/composables/useSanitizedHtml";
import { ErrorType } from "@/primitives/error";
import QuizContent from "@/components/lesson-player/QuizContent.vue";
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
const { pages, currentPage, loading, lastVisitedPageId } = storeToRefs(lessonPageStore);
const { sanitize } = useSanitizedHtml();

// Currently displayed slide's title, for the breadcrumb above the reveal
// container (lastVisitedPageId tracks the active slide, see visitPage below)
const currentSlideTitle = computed(
  () => pages.value.find((p) => p.id === lastVisitedPageId.value)?.title ?? pages.value[0]?.title,
);

// Slide colors follow the app theme (reveal's own black theme would render
// light/white headings on the light slide background otherwise)
const vuetifyTheme = useTheme();
const slideBg = computed(() =>
  vuetifyTheme.current.value.dark
    ? vuetifyTheme.current.value.colors.background
    : vuetifyTheme.current.value.colors.surface,
);
const slideFg = computed(() =>
  vuetifyTheme.current.value.dark
    ? vuetifyTheme.current.value.colors["on-background"]
    : vuetifyTheme.current.value.colors["on-surface"],
);
const controlsColor = computed(() => vuetifyTheme.current.value.colors.primary);
const breadcrumbColor = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "99");

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
  <div class="lesson-player-root">
    <div v-if="!loading && currentPage" class="lesson-breadcrumb">
      {{ currentPage.lessonTitle }} / {{ currentSlideTitle }}
    </div>
    <div ref="revealRef" class="reveal">
      <div class="slides">
        <template v-if="!loading">
          <section
            v-for="page in pages"
            :key="page.id"
            class="lesson-slide"
          >
            <v-card variant="elevated" rounded="lg" class="slide-card">
              <template v-if="page.contentType === 'Image' && page.mediaUrl">
                <div class="slide-media">
                  <img :src="page.mediaUrl" :alt="page.title" />
                </div>
                <!-- LessonPage has no dedicated caption field; Content doubles as caption -->
                <v-card-text
                  class="slide-caption"
                  :innerHTML="sanitize(page.content)"
                />
              </template>

              <template v-else-if="page.contentType === 'Video' && page.mediaUrl">
                <div class="slide-media">
                  <video controls>
                    <source :src="page.mediaUrl" type="video/mp4" />
                    Your browser does not support the video tag.
                  </video>
                </div>
                <v-card-text
                  class="slide-caption"
                  :innerHTML="sanitize(page.content)"
                />
              </template>

              <template v-else-if="page.contentType === 'Embed' && page.mediaUrl">
                <div class="embed-frame">
                  <iframe
                    :src="page.mediaUrl"
                    :title="page.title"
                    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                    allowfullscreen
                  />
                </div>
                <v-card-text
                  class="slide-caption"
                  :innerHTML="sanitize(page.content)"
                />
              </template>

              <v-card-text v-else-if="page.contentType === 'Quiz'">
                <QuizContent :content="page.content ?? ''" />
              </v-card-text>

              <v-card-text
                v-else
                class="slide-content"
                :innerHTML="sanitize(page.content)"
              />
            </v-card>
          </section>
        </template>
        <section v-else>
          <v-progress-circular indeterminate color="primary" />
        </section>
      </div>
    </div>
  </div>
</template>

<style scoped>
.lesson-player-root {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
}

.lesson-breadcrumb {
  flex-shrink: 0;
  padding: 12px 40px 0;
  font-size: 13px;
  color: v-bind(breadcrumbColor);
}

.reveal {
  width: 100%;
  flex: 1;
  min-height: 0;
}

.slides {
  width: 100%;
  height: 100%;
}

.lesson-slide {
  text-align: left;
  padding: 40px;
  background: v-bind(slideBg);
  min-height: 100%;
  height: 100%;
  overflow-y: auto;
}

.slide-card {
  max-width: 900px;
  margin: 0 auto;
}

.slide-content,
.slide-caption {
  font-size: 18px;
  line-height: 1.6;
  color: v-bind(slideFg);
  word-wrap: break-word;
  overflow-wrap: break-word;
}

.slide-caption {
  font-size: 15px;
}

/* Content HTML comes from v-html (no scoped data-attr); reveal's black theme
   would otherwise paint headings white on the light slide background */
.slide-card :deep(h1),
.slide-card :deep(h2),
.slide-card :deep(h3),
.slide-card :deep(h4),
.slide-card :deep(h5),
.slide-card :deep(h6) {
  color: v-bind(slideFg);
}

/* Reveal's theme sizes headings for full slides; captions need modest ones */
.slide-caption :deep(h1),
.slide-caption :deep(h2),
.slide-caption :deep(h3) {
  font-size: 1.15em;
  margin-bottom: 8px;
}

/* Reveal's native controls/progress ship with the theme's #42affa link color */
.reveal :deep(.controls) {
  color: v-bind(controlsColor);
}

.reveal :deep(.progress) {
  color: v-bind(controlsColor);
}

.slide-media {
  display: flex;
  justify-content: center;
  padding: 16px 16px 0;
}

.slide-media img,
.slide-media video {
  max-width: 100%;
  height: auto;
  border-radius: 8px;
}

.embed-frame {
  aspect-ratio: 16 / 9;
  width: 100%;
}

.embed-frame iframe {
  width: 100%;
  height: 100%;
  border: 0;
}
</style>
