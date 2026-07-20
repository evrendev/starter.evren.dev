<script setup lang="ts">
import { useTheme } from "vuetify";
import { usePageStore } from "@/stores/page";
import { useSanitizedHtml } from "@/composables/useSanitizedHtml";
import { ErrorType } from "@/primitives/error";
import { contentTypeIcons } from "@/utils/contentTypeIcons";
import QuizContent from "@/components/lesson-player/QuizContent.vue";
// @ts-ignore - reveal.js type definitions not available
import Reveal from "reveal.js";
import "reveal.js/dist/reveal.css";
import "reveal.js/dist/theme/black.css";

const props = defineProps<{
  chapterId: string;
  // Disable when hosted inside a dialog so reveal.js does not rewrite the page URL hash
  hashNavigation?: boolean;
  // Shrinks the header icon box to match the mobile CSS below (numeric
  // v-icon sizes render as an inline style, which a media query can't win against)
  mobile?: boolean;
}>();

const emit = defineEmits<{
  (e: "ready", instance: typeof Reveal): void;
  (e: "forbidden", message: string): void;
}>();

const pageStore = usePageStore();
const { pages, currentPage, loading, lastVisitedPageId } = storeToRefs(pageStore);
const { sanitize } = useSanitizedHtml();

// Currently displayed slide's title, for the breadcrumb above the reveal
// container (lastVisitedPageId tracks the active slide, see visitPage below)
const currentSlideTitle = computed(
  () => pages.value.find((p) => p.id === lastVisitedPageId.value)?.title ?? pages.value[0]?.title,
);

// Slide colors follow the app theme (reveal's own black theme would render
// light/white headings on the light slide background otherwise)
const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
// Page-level layer, distinct from the card's own "surface" background so the
// card visually separates from its surroundings. Dark already used its own
// "background" token here (dark surface, #3A3D5A, is lighter than dark
// background, #2B2D42); light previously reused "surface" for both layers,
// which made the slide and its cards merge into one flat white area
const slideBg = computed(() => vuetifyTheme.current.value.colors.background);
const slideFg = computed(() =>
  vuetifyTheme.current.value.dark
    ? vuetifyTheme.current.value.colors["on-background"]
    : vuetifyTheme.current.value.colors["on-surface"],
);
// The card's own layer, kept explicit rather than relying on v-card's default
// (which also resolves to "surface", but implicitly)
const cardBg = computed(() => vuetifyTheme.current.value.colors.surface);
const controlsColor = computed(() => vuetifyTheme.current.value.colors.primary);
const breadcrumbColor = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "99");
// Same near-black-in-light / on-surface-in-dark reasoning as LessonSidebar's
// lesson title (light's own on-surface token reads too light against white)
const pageTitleColor = computed(() =>
  vuetifyTheme.current.value.dark
    ? vuetifyTheme.current.value.colors["on-surface"]
    : vuetifyTheme.current.value.colors["grey-900"],
);
const iconBoxBg = computed(() => vuetifyTheme.current.value.colors.accent);
const iconBoxColor = computed(() => vuetifyTheme.current.value.colors["on-accent"]);

// Seed content commonly repeats the page's own title as its first heading;
// strip that one heading when it duplicates page.title so it isn't shown
// twice (once in the new header block, once inside the rendered content)
const stripDuplicateHeading = (html: string | undefined, title: string): string => {
  if (!html) return "";
  const doc = new DOMParser().parseFromString(html, "text/html");
  const first = doc.body.firstElementChild;
  if (first && /^H[1-6]$/.test(first.tagName) && first.textContent?.trim() === title.trim()) {
    first.remove();
  }
  return doc.body.innerHTML;
};

const renderedContent = (page: { content?: string; title: string }) =>
  sanitize(stripDuplicateHeading(page.content, page.title));

const revealRef = ref<HTMLDivElement>();
let revealInstance: typeof Reveal | null = null;

const visitPage = async (slideIndex: number) => {
  const page = pages.value[slideIndex];
  if (!page) return;

  pageStore.lastVisitedPageId = page.id;

  if (!page.completed) {
    await pageStore.markPageCompleted(page.id);
  }
};

onMounted(async () => {
  const result = await pageStore.getChapterPlayer(props.chapterId);

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
      // 0.1 (reveal's original value here) left ~40px of unwanted empty
      // space above the content and shrank the card noticeably narrower
      // than the available width; 0.02 keeps a small viewing margin only
      margin: 0.02,
      // ESC is bound to reveal's overview mode by default; free it so the
      // surrounding v-dialog can handle ESC-to-close
      keyboard: { 27: null },
      touch: true,
      // reveal.js auto-switches to a continuous-scroll view (no .present
      // class, .slide()/.next()/.prev() become no-ops) below this width —
      // default 435px, which our mobile layout (390px test viewport) was
      // silently triggering, breaking the segment bar and toolbar nav
      scrollActivationWidth: 0,
      // Mobile has its own prev/next buttons in LessonMobileToolbar; reveal's
      // native on-screen arrows would overlap and intercept clicks on it.
      // Desktop keeps them (already themed, see the .controls rule below)
      controls: !props.mobile,
    });

    await revealInstance.initialize();

    // Resume at the last visited page when available
    const lastVisitedIndex = pages.value.findIndex(
      (p) => p.id === pageStore.lastVisitedPageId,
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
      {{ currentPage.chapterTitle }} /
      <span class="breadcrumb-current">{{ currentSlideTitle }}</span>
    </div>
    <div ref="revealRef" class="reveal">
      <div class="slides">
        <template v-if="!loading">
          <section
            v-for="page in pages"
            :key="page.id"
            class="lesson-slide"
          >
            <div class="page-header-row">
              <div class="header-icon-box">
                <v-icon
                  :icon="contentTypeIcons[page.contentType] ?? 'bx-align-left'"
                  :size="mobile ? 18 : 24"
                />
              </div>
              <!-- div, not h1: reveal.js's theme CSS targets any h1-h6 inside
                   .reveal with forced uppercase/letter-spacing/text-shadow -->
              <div class="page-header-title" role="heading" aria-level="1">{{ page.title }}</div>
            </div>

            <v-card
              variant="elevated"
              rounded="lg"
              class="slide-card"
              :class="{ 'slide-card--elevated': !isDark }"
            >
              <template v-if="page.contentType === 'Image' && page.mediaUrl">
                <div class="slide-media">
                  <img :src="page.mediaUrl" :alt="page.title" />
                </div>
                <!-- Page has no dedicated caption field; Content doubles as caption -->
                <v-card-text
                  class="slide-caption"
                  :innerHTML="renderedContent(page)"
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
                  :innerHTML="renderedContent(page)"
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
                  :innerHTML="renderedContent(page)"
                />
              </template>

              <template v-else-if="page.isImported">
                <div class="imported-frame-wrap">
                  <iframe
                    :srcdoc="page.content"
                    sandbox="allow-same-origin"
                    :title="page.title"
                    class="imported-frame"
                  />
                </div>
              </template>

              <v-card-text v-else-if="page.contentType === 'Quiz'">
                <QuizContent :content="page.content ?? ''" />
              </v-card-text>

              <v-card-text
                v-else
                class="slide-content"
                :innerHTML="renderedContent(page)"
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
  padding: 12px 40px 4px;
  font-size: 13px;
  color: v-bind(breadcrumbColor);
}

/* Bold, full-opacity last segment ("you are here"); segments before it stay
   in the breadcrumb's own muted color via the CSS default */
.breadcrumb-current {
  color: v-bind(pageTitleColor);
  font-weight: 600;
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
  padding: 16px 20px 40px;
  background: v-bind(slideBg);
  min-height: 100%;
  height: 100%;
  overflow-y: auto;
}

.page-header-row {
  display: flex;
  align-items: center;
  gap: 16px;
  max-width: 1400px;
  margin: 0 auto 16px;
}

.header-icon-box {
  flex-shrink: 0;
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 10px;
  background: v-bind(iconBoxBg);
  color: v-bind(iconBoxColor);
}

.page-header-title {
  font-size: 26px;
  font-weight: 700;
  line-height: 1.25;
  color: v-bind(pageTitleColor);
}

.slide-card {
  max-width: 1400px;
  margin: 0 auto;
  background: v-bind(cardBg);
}

/* Light only (per design): dark's card already separates from its slide
   background via the surface/background token contrast, no shadow needed */
.slide-card--elevated {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
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

/* Matches the width/height config passed to pptxToHtml (960x540) so the sandboxed
   iframe's rich content isn't scaled/cropped unexpectedly */
.imported-frame-wrap {
  aspect-ratio: 960 / 540;
  width: 100%;
  max-width: 1400px;
  margin: 0 auto;
  background: #fff;
  border-radius: 4px;
  overflow: hidden;
}

.imported-frame {
  width: 100%;
  height: 100%;
  border: 0;
}

/* Matches the JS mobile threshold (useDisplay().smAndDown, <960px) used to
   switch LessonPlayerDialog into the mobile "story mode" layout */
@media (max-width: 960px) {
  .lesson-breadcrumb {
    padding: 8px 16px 2px;
    font-size: 12px;
  }

  .lesson-slide {
    padding: 12px 16px 32px;
  }

  .page-header-row {
    gap: 10px;
    margin-bottom: 12px;
  }

  .header-icon-box {
    width: 36px;
    height: 36px;
    border-radius: 8px;
  }

  .page-header-title {
    font-size: 19px;
  }
}
</style>
