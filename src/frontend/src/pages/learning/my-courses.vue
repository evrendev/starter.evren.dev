<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useTheme } from "vuetify";
import { formatDistanceToNow } from "date-fns";
import { enUS, tr, de } from "date-fns/locale";
import { useCourseEnrollmentStore } from "@/stores/courseEnrollment";
import { useCourseStore } from "@/stores/course";
import { useChapterStore } from "@/stores/chapter";
import { usePageStore } from "@/stores/page";
import LessonPlayerDialog from "@/components/lesson-player/LessonPlayerDialog.vue";

const { t, locale } = useI18n();

const BASE_URL = import.meta.env.VITE_APP_BACKEND_BASE_URL;

const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
// Same red in both themes (dark theme swaps primary/secondary, see
// LessonSidebar's isDark ? 'secondary' : 'primary' precedent).
const redColor = computed(() => (isDark.value ? "secondary" : "primary"));

const dateFnsLocales = { en: enUS, tr, de } as const;

const enrollmentStore = useCourseEnrollmentStore();
const { enrollments, loading } = storeToRefs(enrollmentStore);

// CourseEnrollmentDto has no Image field, only the course list does (same
// data catalog.vue already loads) — fetched once on mount and looked up by
// courseId for the cover placeholder below.
const courseStore = useCourseStore();
const courseImages = computed<Record<string, string | undefined>>(() =>
  Object.fromEntries(
    courseStore.items.map((course) => [course.id, course.image as unknown as string | undefined]),
  ),
);

const chapterStore = useChapterStore();
const pageStore = usePageStore();

interface NextChapter {
  chapterId: string;
  title: string;
}

interface CourseCardInfo {
  chapterCount: number;
  nextChapter: NextChapter | null;
}

// Keyed by courseId — resolved once on load (and after the player dialog
// closes) by walking each course's chapters and looking up per-chapter
// percentComplete, same algorithm catalog.vue's handleContinue uses for a
// single course. Doing this eagerly for every enrolled course is N+1-shaped
// (1 chapter-list call + 1 percentComplete call per chapter, per course) —
// flagged in the Task P1 report as a candidate for a combined backend
// endpoint if the enrolled-course count grows large in practice.
const courseCardInfo = ref<Record<string, CourseCardInfo>>({});
const cardInfoLoading = ref(false);

const loadCourseCardInfo = async () => {
  cardInfoLoading.value = true;
  const info: Record<string, CourseCardInfo> = {};

  try {
    // Sequential on purpose: chapterStore.items is shared mutable state:
    // running these in parallel would let concurrent courses clobber each
    // other's chapter list mid-flight.
    for (const enrollment of enrollments.value) {
      chapterStore.setFilters({ search: null, courseId: enrollment.courseId });
      await chapterStore.getPaginatedItems();
      const chapters = chapterStore.items;

      let nextChapter: NextChapter | null = null;

      // Chapter titles are already formatted "Chapter N - Name" by convention
      // (confirmed against real data), so the message just prepends "Next:"
      // to the title as-is — no separate index needs to be injected.
      for (const chapter of chapters) {
        const result = await pageStore.getChapterPlayer(chapter.id);
        if (result.succeeded && (result.data?.percentComplete || 0) < 100) {
          nextChapter = { chapterId: chapter.id, title: chapter.title };
          break;
        }
      }

      info[enrollment.courseId] = {
        chapterCount: chapters.length,
        nextChapter,
      };
    }
  } finally {
    courseCardInfo.value = info;
    cardInfoLoading.value = false;
  }
};

onMounted(async () => {
  courseStore.setFilters({ search: null, categoryId: null, published: true });
  await Promise.all([
    enrollmentStore.getMyEnrollments(),
    courseStore.getPaginatedItems(),
  ]);
  await loadCourseCardInfo();
});

const enrolledCount = computed(() => enrollments.value.length);
const inProgressCount = computed(
  () =>
    enrollments.value.filter(
      (e) => e.percentComplete > 0 && e.percentComplete < 100,
    ).length,
);
const completedCount = computed(
  () => enrollments.value.filter((e) => e.percentComplete >= 100).length,
);

const enrolledAgo = (enrolledAt: string) =>
  formatDistanceToNow(new Date(enrolledAt), {
    addSuffix: true,
    locale: dateFnsLocales[locale.value as keyof typeof dateFnsLocales] ?? enUS,
  });

const buttonLabel = (percentComplete: number) => {
  if (percentComplete >= 100) return t("learning.myCourses.review");
  if (percentComplete > 0) return t("learning.myCourses.continue");
  return t("learning.myCourses.start");
};

const playerChapterId = ref<string | null>(null);
const playerCategoryTitle = ref<string | null>(null);
const playerOpen = ref(false);
// Set only while resolving the target chapter for a click, mirrors
// catalog.vue's resolvingCourseId so the button can show a loading state.
const resolvingCourseId = ref<string | null>(null);

const openCourse = async (courseId: string, categoryTitle?: string) => {
  resolvingCourseId.value = courseId;

  try {
    const info = courseCardInfo.value[courseId];
    chapterStore.setFilters({ search: null, courseId });
    await chapterStore.getPaginatedItems();
    const chapters = chapterStore.items;
    if (chapters.length === 0) return;

    const targetChapterId = info?.nextChapter?.chapterId ?? chapters[0].id;

    playerChapterId.value = targetChapterId;
    playerCategoryTitle.value = categoryTitle ?? null;
    playerOpen.value = true;
  } finally {
    resolvingCourseId.value = null;
  }
};

watch(playerOpen, async (open) => {
  if (!open) {
    await enrollmentStore.getMyEnrollments();
    await loadCourseCardInfo();
  }
});
</script>

<template>
  <v-container>
    <h2 class="mb-4">{{ t("learning.myCourses.title") }}</h2>

    <v-progress-circular
      v-if="loading && enrollments.length === 0"
      indeterminate
      :color="redColor"
      class="d-block mx-auto mt-8"
    />

    <div v-else-if="enrollments.length === 0" class="text-center py-16">
      <v-avatar :color="redColor" variant="tonal" size="64" class="mb-4">
        <v-icon icon="bx-book-reader" size="32" />
      </v-avatar>
      <p class="text-h6 mb-1">{{ t("learning.myCourses.empty.title") }}</p>
      <p class="text-body-2 text-medium-emphasis mb-6">
        {{ t("learning.myCourses.empty.subtitle") }}
      </p>
      <v-btn
        :color="redColor"
        variant="flat"
        :to="{ name: 'learning-catalog' }"
      >
        {{ t("learning.myCourses.empty.browseCatalog") }}
      </v-btn>
    </div>

    <template v-else>
      <v-row class="mb-2">
        <v-col cols="12" sm="4">
          <v-card
            variant="flat"
            class="stat-card d-flex align-center pa-4"
            :class="{ 'stat-card--elevated': !isDark }"
          >
            <v-avatar :color="redColor" variant="tonal" size="40" class="me-4">
              <v-icon icon="bx-collection" />
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ enrolledCount }}</div>
              <div class="text-caption text-medium-emphasis">
                {{ t("learning.myCourses.stats.enrolled") }}
              </div>
            </div>
          </v-card>
        </v-col>
        <v-col cols="12" sm="4">
          <v-card
            variant="flat"
            class="stat-card d-flex align-center pa-4"
            :class="{ 'stat-card--elevated': !isDark }"
          >
            <v-avatar color="warning" variant="tonal" size="40" class="me-4">
              <v-icon icon="bx-time-five" />
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ inProgressCount }}</div>
              <div class="text-caption text-medium-emphasis">
                {{ t("learning.myCourses.stats.inProgress") }}
              </div>
            </div>
          </v-card>
        </v-col>
        <v-col cols="12" sm="4">
          <v-card
            variant="flat"
            class="stat-card d-flex align-center pa-4"
            :class="{ 'stat-card--elevated': !isDark }"
          >
            <v-avatar color="success" variant="tonal" size="40" class="me-4">
              <v-icon icon="bx-check-circle" />
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ completedCount }}</div>
              <div class="text-caption text-medium-emphasis">
                {{ t("learning.myCourses.stats.completed") }}
              </div>
            </div>
          </v-card>
        </v-col>
      </v-row>

      <v-row>
        <v-col
          v-for="enrollment in enrollments"
          :key="enrollment.courseId"
          cols="12"
          sm="6"
          md="4"
        >
          <v-card
            class="h-100 d-flex flex-column"
            :variant="isDark ? 'outlined' : 'elevated'"
          >
            <div v-if="courseImages[enrollment.courseId]" class="course-cover">
              <img
                :src="`${BASE_URL}/${courseImages[enrollment.courseId]}`"
                :alt="enrollment.courseTitle"
              />
            </div>
            <div v-else class="course-cover course-cover--placeholder">
              <v-icon icon="bx-image" size="28" />
              <span class="text-caption mt-1">{{
                t("learning.myCourses.courseCover")
              }}</span>
            </div>

            <v-card-item>
              <v-chip
                v-if="enrollment.percentComplete >= 100"
                color="success"
                variant="flat"
                size="small"
                class="mb-2"
              >
                {{ t("learning.myCourses.completedChip") }}
              </v-chip>
              <v-chip
                v-else-if="enrollment.categoryTitle"
                variant="tonal"
                size="small"
                class="mb-2"
              >
                {{ enrollment.categoryTitle }}
              </v-chip>

              <v-card-title class="px-0 text-wrap" style="white-space: normal">{{
                enrollment.courseTitle
              }}</v-card-title>
            </v-card-item>

            <v-card-text class="flex-grow-1">
              <div
                class="d-flex align-center text-caption text-medium-emphasis mb-3"
              >
                <v-icon icon="bx-book-open" size="14" class="me-1" />
                <span class="me-3">
                  {{
                    t("learning.myCourses.chapterCount", {
                      count: courseCardInfo[enrollment.courseId]?.chapterCount ?? 0,
                    })
                  }}
                </span>
                <v-icon icon="bx-calendar" size="14" class="me-1" />
                <span>
                  {{
                    t("learning.myCourses.enrolledAgo", {
                      time: enrolledAgo(enrollment.enrolledAt),
                    })
                  }}
                </span>
              </div>

              <v-progress-linear
                :model-value="enrollment.percentComplete"
                :color="enrollment.percentComplete >= 100 ? 'success' : redColor"
                height="6"
                rounded
                class="mb-2"
              />

              <div class="text-caption">
                <span v-if="cardInfoLoading">&nbsp;</span>
                <span
                  v-else-if="courseCardInfo[enrollment.courseId]?.nextChapter"
                >
                  {{
                    t("learning.myCourses.next", {
                      title: courseCardInfo[enrollment.courseId]!.nextChapter!.title,
                    })
                  }}
                </span>
                <span v-else>{{ t("learning.myCourses.allChaptersCompleted") }}</span>
              </div>
            </v-card-text>

            <v-card-actions>
              <v-btn
                :color="redColor"
                variant="flat"
                block
                :loading="resolvingCourseId === enrollment.courseId"
                @click="openCourse(enrollment.courseId, enrollment.categoryTitle)"
              >
                {{ buttonLabel(enrollment.percentComplete) }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </template>

    <LessonPlayerDialog
      v-model="playerOpen"
      :chapter-id="playerChapterId"
      :category-title="playerCategoryTitle"
    />
  </v-container>
</template>

<style scoped>
.stat-card {
  background: rgb(var(--v-theme-surface));
}

/* Light only (per design, same precedent as LessonPlayer's
   .slide-card--elevated): dark's card already separates from the page
   background via the surface/background token contrast, no shadow needed */
.stat-card--elevated {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.course-cover {
  height: 140px;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.course-cover--placeholder {
  flex-direction: column;
  background: rgba(var(--v-theme-on-surface), 0.05);
  color: rgba(var(--v-theme-on-surface), 0.4);
}

.course-cover img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>
