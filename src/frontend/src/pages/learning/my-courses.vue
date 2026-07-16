<script setup lang="ts">
import { useCourseEnrollmentStore } from "@/stores/courseEnrollment";
import { useChapterStore } from "@/stores/chapter";
import { useLessonStore } from "@/stores/lesson";
import { useLessonPageStore } from "@/stores/lessonPage";
import LessonPlayerDialog from "@/components/lesson-player/LessonPlayerDialog.vue";

const { t } = useI18n();

const enrollmentStore = useCourseEnrollmentStore();
const { enrollments, loading } = storeToRefs(enrollmentStore);

const chapterStore = useChapterStore();
const { items: chapters, loading: chaptersLoading } =
  storeToRefs(chapterStore);

const lessonStore = useLessonStore();
const { items: lessons, loading: lessonsLoading } = storeToRefs(lessonStore);

const lessonPageStore = useLessonPageStore();

const expandedCourse = ref<string>();
const expandedChapter = ref<string>();
const lessonProgress = ref<Record<string, number>>({});
const lessonProgressLoading = ref(false);

onMounted(async () => {
  await enrollmentStore.getMyEnrollments();
});

const handleCourseExpand = async (courseId: string) => {
  expandedChapter.value = undefined;
  chapterStore.setFilters({ search: null, courseId });
  await chapterStore.getPaginatedItems();
};

const handleChapterExpand = async (chapterId: string) => {
  lessonProgress.value = {};
  lessonStore.setFilters({ search: null, chapterId });
  await lessonStore.getPaginatedItems();

  lessonProgressLoading.value = true;
  try {
    await Promise.all(
      lessons.value.map(async (lesson) => {
        const result = await lessonPageStore.getLessonPlayer(lesson.id);
        if (result.succeeded && result.data) {
          lessonProgress.value[lesson.id] = result.data.percentComplete || 0;
        }
      }),
    );
  } finally {
    lessonProgressLoading.value = false;
  }
};

const playerLessonId = ref<string | null>(null);
const playerCategoryTitle = ref<string | null>(null);
const playerOpen = ref(false);

const goToLesson = (lessonId: string, categoryTitle?: string) => {
  playerLessonId.value = lessonId;
  playerCategoryTitle.value = categoryTitle ?? null;
  playerOpen.value = true;
};

const refreshProgress = async () => {
  await enrollmentStore.getMyEnrollments();

  lessonProgressLoading.value = true;
  try {
    await Promise.all(
      lessons.value.map(async (lesson) => {
        const result = await lessonPageStore.getLessonPlayer(lesson.id);
        if (result.succeeded && result.data) {
          lessonProgress.value[lesson.id] = result.data.percentComplete || 0;
        }
      }),
    );
  } finally {
    lessonProgressLoading.value = false;
  }
};

watch(playerOpen, async (open) => {
  if (!open) {
    await refreshProgress();
  }
});
</script>

<template>
  <v-container>
    <h2 class="mb-4">{{ t("learning.myCourses.title") }}</h2>

    <!-- Only on initial load; background refreshes (e.g. after closing the
         player dialog) must not unmount the panels and lose expansion state -->
    <v-progress-circular
      v-if="loading && enrollments.length === 0"
      indeterminate
      color="primary"
      class="d-block mx-auto mt-8"
    />

    <v-alert v-else-if="enrollments.length === 0" type="info" variant="tonal">
      {{ t("learning.myCourses.empty") }}
    </v-alert>

    <v-expansion-panels v-else v-model="expandedCourse">
      <v-expansion-panel
        v-for="enrollment in enrollments"
        :key="enrollment.courseId"
        :value="enrollment.courseId"
        @group:selected="handleCourseExpand(enrollment.courseId)"
      >
        <v-expansion-panel-title>
          <div class="d-flex flex-column" style="width: 100%">
            <span class="font-weight-bold">{{ enrollment.courseTitle }}</span>
            <v-progress-linear
              :model-value="enrollment.percentComplete"
              color="primary"
              height="6"
              class="mt-2"
            />
          </div>
        </v-expansion-panel-title>

        <v-expansion-panel-text>
          <v-progress-circular
            v-if="chaptersLoading"
            indeterminate
            color="primary"
            size="24"
          />

          <v-expansion-panels
            v-else
            v-model="expandedChapter"
            variant="accordion"
          >
            <v-expansion-panel
              v-for="chapter in chapters"
              :key="chapter.id"
              :value="chapter.id"
              @group:selected="handleChapterExpand(chapter.id)"
            >
              <v-expansion-panel-title>{{
                chapter.title
              }}</v-expansion-panel-title>
              <v-expansion-panel-text>
                <v-progress-circular
                  v-if="lessonsLoading"
                  indeterminate
                  color="primary"
                  size="20"
                />

                <v-list v-else density="compact">
                  <v-list-item
                    v-for="lesson in lessons"
                    :key="lesson.id"
                    class="cursor-pointer"
                    @click="goToLesson(lesson.id, enrollment.categoryTitle)"
                  >
                    <template #prepend>
                      <v-icon
                        :icon="
                          (lessonProgress[lesson.id] || 0) >= 100
                            ? 'mdi-check-circle'
                            : (lessonProgress[lesson.id] || 0) > 0
                              ? 'mdi-progress-clock'
                              : 'mdi-play-circle-outline'
                        "
                        :color="
                          (lessonProgress[lesson.id] || 0) >= 100
                            ? 'success'
                            : (lessonProgress[lesson.id] || 0) > 0
                              ? 'warning'
                              : 'grey'
                        "
                        size="small"
                      />
                    </template>
                    <v-list-item-title>{{ lesson.title }}</v-list-item-title>
                    <v-list-item-subtitle v-if="!lessonProgressLoading">
                      {{ lessonProgress[lesson.id] || 0 }}%
                      {{ t("learning.myCourses.percentComplete") }}
                    </v-list-item-subtitle>
                  </v-list-item>
                </v-list>
              </v-expansion-panel-text>
            </v-expansion-panel>
          </v-expansion-panels>
        </v-expansion-panel-text>
      </v-expansion-panel>
    </v-expansion-panels>

    <LessonPlayerDialog
      v-model="playerOpen"
      :lesson-id="playerLessonId"
      :category-title="playerCategoryTitle"
    />
  </v-container>
</template>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}
</style>
