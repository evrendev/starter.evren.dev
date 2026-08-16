<script setup lang="ts">
import { useCourseStore } from "@/stores/course";
import { useCourseEnrollmentStore } from "@/stores/courseEnrollment";
import { useChapterStore } from "@/stores/chapter";
import { usePageStore } from "@/stores/page";
import { Notify } from "@/stores/notification";
import LessonPlayerDialog from "@/components/lesson-player/LessonPlayerDialog.vue";

const { t } = useI18n();

const courseStore = useCourseStore();
const { items: courses, loading } = storeToRefs(courseStore);

const enrollmentStore = useCourseEnrollmentStore();
const { enrollments, loading: enrollmentLoading } =
  storeToRefs(enrollmentStore);

const chapterStore = useChapterStore();
const pageStore = usePageStore();

const enrollingCourseId = ref<string | null>(null);
// Set only while resolving which chapter "Continue" should open — separate
// from playerOpen so the button can show a loading state without the dialog
// flashing open before a chapterId is actually known
const resolvingCourseId = ref<string | null>(null);

onMounted(async () => {
  courseStore.setFilters({ search: null, categoryId: null, published: true });
  await Promise.all([
    courseStore.getPaginatedItems(),
    enrollmentStore.getMyEnrollments(),
  ]);
});

const getEnrollment = (courseId: string) =>
  enrollments.value.find((e) => e.courseId === courseId);

const handleEnroll = async (courseId: string) => {
  enrollingCourseId.value = courseId;

  try {
    const result = await enrollmentStore.enrollInCourse(courseId);

    if (result.succeeded) {
      Notify.success(t("learning.catalog.notifications.enrolled"));
      await enrollmentStore.getMyEnrollments();
    } else {
      Notify.error(t("learning.catalog.notifications.enrollFailed"));
    }
  } finally {
    enrollingCourseId.value = null;
  }
};

const playerChapterId = ref<string | null>(null);
const playerCategoryTitle = ref<string | null>(null);
const playerOpen = ref(false);

// "Continue" has no chapter context (this card is course-level) — resolve one
// by picking the first chapter (by Order) that isn't 100% complete yet, i.e.
// wherever the learner last left off; if the whole course is done, reopen the
// first chapter for review. Same per-chapter percentComplete lookup my-courses
// uses, just run once on click instead of eagerly for every chapter.
const handleContinue = async (courseId: string, categoryTitle?: string) => {
  resolvingCourseId.value = courseId;

  try {
    chapterStore.setFilters({ search: null, courseId });
    await chapterStore.getPaginatedItems();
    // Chapter has no Order field on the frontend model — trust the backend's
    // own pagination ordering (same list my-courses.vue already renders as-is)
    const chapters = chapterStore.items;

    if (chapters.length === 0) return;

    let targetChapterId = chapters[0].id;

    for (const chapter of chapters) {
      const result = await pageStore.getChapterPlayer(chapter.id);
      if (result.succeeded && (result.data?.percentComplete || 0) < 100) {
        targetChapterId = chapter.id;
        break;
      }
    }

    playerChapterId.value = targetChapterId;
    playerCategoryTitle.value = categoryTitle ?? null;
    playerOpen.value = true;
  } finally {
    resolvingCourseId.value = null;
  }
};
</script>

<template>
  <v-container>
    <h2 class="mb-4">{{ t("learning.catalog.title") }}</h2>

    <v-progress-circular
      v-if="loading || enrollmentLoading"
      indeterminate
      color="primary"
      class="d-block mx-auto mt-8"
    />

    <v-row v-else>
      <v-col
        v-for="course in courses"
        :key="course.id"
        cols="12"
        sm="6"
        md="4"
      >
        <v-card class="h-100 d-flex flex-column">
          <v-card-title>{{ course.title }}</v-card-title>
          <v-card-subtitle v-if="course.categoryTitle">
            {{ course.categoryTitle }}
          </v-card-subtitle>

          <v-card-text class="flex-grow-1">
            <p class="text-body-2">{{ course.introduction }}</p>

            <template v-if="getEnrollment(course.id)">
              <v-progress-linear
                :model-value="getEnrollment(course.id)?.percentComplete || 0"
                color="primary"
                height="8"
                class="mt-4 mb-1"
              />
              <div class="text-caption">
                {{ getEnrollment(course.id)?.percentComplete || 0 }}%
                {{ t("learning.myCourses.percentComplete") }}
              </div>
            </template>
          </v-card-text>

          <v-card-actions>
            <v-btn
              v-if="getEnrollment(course.id)"
              color="primary"
              variant="flat"
              block
              :loading="resolvingCourseId === course.id"
              @click="handleContinue(course.id, course.categoryTitle)"
            >
              {{ t("learning.catalog.continue") }}
            </v-btn>
            <v-btn
              v-else
              color="primary"
              variant="outlined"
              block
              :loading="enrollingCourseId === course.id"
              @click="handleEnroll(course.id)"
            >
              {{ t("learning.catalog.enroll") }}
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <LessonPlayerDialog
      v-model="playerOpen"
      :chapter-id="playerChapterId"
      :category-title="playerCategoryTitle"
    />
  </v-container>
</template>
