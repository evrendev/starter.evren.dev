<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useTheme } from "vuetify";
import { useCourseStore } from "@/stores/course";
import { useCategoryStore } from "@/stores/category";
import { useCourseEnrollmentStore } from "@/stores/courseEnrollment";
import { Notify } from "@/stores/notification";
import LessonPlayerDialog from "@/components/lesson-player/LessonPlayerDialog.vue";
import CourseCover from "@/components/learning/CourseCover.vue";
import ElevatedCard from "@/components/shared/ElevatedCard.vue";
import CheckoutDialog from "@/components/learning/CheckoutDialog.vue";

const { t } = useI18n();

const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
// Same red in both themes (dark theme swaps primary/secondary tokens, see
// my-courses.vue / LessonSidebar's isDark ? 'secondary' : 'primary' precedent).
const redColor = computed(() => (isDark.value ? "secondary" : "primary"));

const courseStore = useCourseStore();
const { items: courses, loading } = storeToRefs(courseStore);

const categoryStore = useCategoryStore();
const { items: categories } = storeToRefs(categoryStore);

const enrollmentStore = useCourseEnrollmentStore();
const { enrollments, loading: enrollmentLoading } =
  storeToRefs(enrollmentStore);

const search = ref("");
const categoryId = ref<string | null>(null);

type SortOption = "default" | "title-asc" | "title-desc";
// Design has no explicit sort dropdown options visible; CourseDto exposes no
// created-date field to sort a genuine "Newest" by, so this offers the one
// sort that's backed by real, always-present data (title) plus the server's
// own default order — reported in the Task P2 summary rather than guessed.
const sortBy = ref<SortOption>("default");

const fetchCourses = async () => {
  courseStore.setFilters({
    search: search.value || null,
    categoryId: categoryId.value,
    published: true,
  });
  await courseStore.getPaginatedItems();
};

debouncedWatch(search, fetchCourses, { debounce: 350 });
watch(categoryId, fetchCourses);

const sortedCourses = computed(() => {
  if (sortBy.value === "default") return courses.value;
  const collator = new Intl.Collator();
  const sorted = [...courses.value].sort((a, b) =>
    collator.compare(a.title, b.title),
  );
  return sortBy.value === "title-desc" ? sorted.reverse() : sorted;
});

onMounted(async () => {
  await Promise.all([
    categoryStore.getAllItems(),
    fetchCourses(),
    enrollmentStore.getMyEnrollments(),
  ]);
});

const getEnrollment = (courseId: string) =>
  enrollments.value.find((e) => e.courseId === courseId);

const enrollingCourseId = ref<string | null>(null);

const checkoutOpen = ref(false);
const checkoutCourse = ref<{ courseId: string; title: string; amount: number } | null>(null);

const handleEnroll = async (courseId: string) => {
  const course = courses.value.find((c) => c.id === courseId);

  // Paid courses go through PayPal Checkout instead of the direct enroll
  // call — EnrollInCourseRequestHandler itself rejects these with a 409
  // (Task Q1), so routing them here first isn't just a UX nicety.
  if (course?.amount) {
    checkoutCourse.value = { courseId, title: course.title, amount: course.amount };
    checkoutOpen.value = true;
    return;
  }

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

// "Continue" has no chapter context (this card is course-level) — the backend
// already resolves the first not-yet-100%-complete chapter (by Order) per
// enrollment, wherever the learner last left off; nextChapterId is only null
// when the course has zero chapters, since an all-completed course still
// picks its first chapter for review (see MapsterSettings.cs NextChapterId).
const handleContinue = (courseId: string, categoryTitle?: string) => {
  const enrollment = getEnrollment(courseId);
  if (!enrollment?.nextChapterId) return;

  playerChapterId.value = enrollment.nextChapterId;
  playerCategoryTitle.value = categoryTitle ?? null;
  playerOpen.value = true;
};

const buttonLabel = (courseId: string) => {
  const enrollment = getEnrollment(courseId);
  if (!enrollment) return t("learning.catalog.enroll");
  return enrollment.percentComplete >= 100
    ? t("learning.myCourses.review")
    : t("learning.catalog.continue");
};

watch(playerOpen, async (open) => {
  if (!open) {
    await enrollmentStore.getMyEnrollments();
  }
});
</script>

<template>
  <div>
    <h2 class="mb-4">{{ t("learning.catalog.title") }}</h2>

    <ElevatedCard class="pa-4 mb-4">
      <v-row>
        <v-col cols="12" sm="6">
          <v-text-field
            v-model="search"
            density="comfortable"
            variant="outlined"
            prepend-inner-icon="bx-search"
            :placeholder="t('learning.catalog.searchPlaceholder')"
            clearable
            hide-details
          />
        </v-col>
        <v-col cols="6" sm="3">
          <v-select
            v-model="categoryId"
            density="comfortable"
            variant="outlined"
            :items="[
              { title: t('learning.catalog.allCategories'), value: null },
              ...categories.map((c) => ({ title: c.title, value: c.id })),
            ]"
            hide-details
          />
        </v-col>
        <v-col cols="6" sm="3">
          <v-select
            v-model="sortBy"
            density="comfortable"
            variant="outlined"
            :items="[
              { title: t('learning.catalog.sort.default'), value: 'default' },
              { title: t('learning.catalog.sort.titleAsc'), value: 'title-asc' },
              { title: t('learning.catalog.sort.titleDesc'), value: 'title-desc' },
            ]"
            hide-details
          />
        </v-col>
      </v-row>
    </ElevatedCard>

    <v-progress-circular
      v-if="loading || enrollmentLoading"
      indeterminate
      :color="redColor"
      class="d-block mx-auto mt-8"
    />

    <v-alert
      v-else-if="sortedCourses.length === 0"
      type="info"
      variant="tonal"
      class="mt-4"
    >
      {{ t("learning.catalog.noResults") }}
    </v-alert>

    <v-row v-else>
      <v-col
        v-for="course in sortedCourses"
        :key="course.id"
        cols="12"
        sm="6"
        md="4"
      >
        <v-card
          class="h-100 d-flex flex-column"
          :variant="isDark ? 'outlined' : 'elevated'"
        >
          <CourseCover :image="course.image as unknown as string" :alt="course.title" />

          <v-card-item>
            <v-chip
              v-if="course.categoryTitle"
              variant="tonal"
              size="small"
              class="mb-2"
            >
              {{ course.categoryTitle }}
            </v-chip>

            <v-card-title class="px-0 text-wrap" style="white-space: normal">{{
              course.title
            }}</v-card-title>
          </v-card-item>

          <v-card-text class="flex-grow-1">
            <p class="text-body-2 text-truncate-3 mb-3">
              {{ course.description }}
            </p>

            <div class="d-flex align-center justify-space-between">
              <div class="d-flex align-center text-caption text-medium-emphasis">
                <v-icon icon="bx-book-open" size="14" class="me-1" />
                <span>
                  {{
                    t("learning.chapterCount", {
                      count: course.chapterCount ?? 0,
                    })
                  }}
                </span>
              </div>

              <span v-if="!course.amount" class="text-success font-weight-bold">
                {{ t("learning.catalog.free") }}
              </span>
              <span v-else class="font-weight-bold"> {{ course.amount }} € </span>
            </div>

            <template v-if="getEnrollment(course.id)">
              <v-progress-linear
                :model-value="getEnrollment(course.id)?.percentComplete || 0"
                :color="
                  (getEnrollment(course.id)?.percentComplete || 0) >= 100
                    ? 'success'
                    : redColor
                "
                height="6"
                rounded
                class="mt-3"
              />
            </template>
          </v-card-text>

          <v-card-actions>
            <v-btn
              v-if="getEnrollment(course.id)"
              :color="redColor"
              variant="flat"
              block
              @click="handleContinue(course.id, course.categoryTitle)"
            >
              {{ buttonLabel(course.id) }}
            </v-btn>
            <v-btn
              v-else
              :color="redColor"
              variant="flat"
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

    <CheckoutDialog
      v-if="checkoutCourse"
      v-model="checkoutOpen"
      :course="checkoutCourse"
    />
  </div>
</template>

<style scoped>
.text-truncate-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
