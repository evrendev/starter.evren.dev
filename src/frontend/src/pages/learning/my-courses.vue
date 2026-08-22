<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useTheme } from "vuetify";
import { formatDistanceToNow } from "date-fns";
import { enUS, tr, de } from "date-fns/locale";
import { useCourseEnrollmentStore } from "@/stores/courseEnrollment";
import { useCourseStore } from "@/stores/course";
import LessonPlayerDialog from "@/components/lesson-player/LessonPlayerDialog.vue";
import CourseCover from "@/components/learning/CourseCover.vue";
import ElevatedCard from "@/components/shared/ElevatedCard.vue";

const { t, locale } = useI18n();

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

onMounted(async () => {
  courseStore.setFilters({ search: null, categoryId: null, published: true });
  await Promise.all([
    enrollmentStore.getMyEnrollments(),
    courseStore.getPaginatedItems(),
  ]);
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

const openCourse = (courseId: string, categoryTitle?: string) => {
  const enrollment = enrollments.value.find((e) => e.courseId === courseId);
  if (!enrollment?.nextChapterId) return;

  playerChapterId.value = enrollment.nextChapterId;
  playerCategoryTitle.value = categoryTitle ?? null;
  playerOpen.value = true;
};

watch(playerOpen, async (open) => {
  if (!open) {
    await enrollmentStore.getMyEnrollments();
  }
});
</script>

<template>
  <div>
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
          <ElevatedCard class="d-flex align-center pa-4">
            <v-avatar :color="redColor" variant="tonal" size="40" class="me-4">
              <v-icon icon="bx-collection" />
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ enrolledCount }}</div>
              <div class="text-caption text-medium-emphasis">
                {{ t("learning.myCourses.stats.enrolled") }}
              </div>
            </div>
          </ElevatedCard>
        </v-col>
        <v-col cols="12" sm="4">
          <ElevatedCard class="d-flex align-center pa-4">
            <v-avatar color="warning" variant="tonal" size="40" class="me-4">
              <v-icon icon="bx-time-five" />
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ inProgressCount }}</div>
              <div class="text-caption text-medium-emphasis">
                {{ t("learning.myCourses.stats.inProgress") }}
              </div>
            </div>
          </ElevatedCard>
        </v-col>
        <v-col cols="12" sm="4">
          <ElevatedCard class="d-flex align-center pa-4">
            <v-avatar color="success" variant="tonal" size="40" class="me-4">
              <v-icon icon="bx-check-circle" />
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ completedCount }}</div>
              <div class="text-caption text-medium-emphasis">
                {{ t("learning.myCourses.stats.completed") }}
              </div>
            </div>
          </ElevatedCard>
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
            <CourseCover
              :image="courseImages[enrollment.courseId]"
              :alt="enrollment.courseTitle"
            />

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
                    t("learning.chapterCount", {
                      count: enrollment.chapterCount,
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
                <span v-if="enrollment.percentComplete < 100 && enrollment.nextChapterTitle">
                  {{
                    t("learning.myCourses.next", {
                      title: enrollment.nextChapterTitle,
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
  </div>
</template>
