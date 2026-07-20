<script setup lang="ts">
import { useCourseEnrollmentStore } from "@/stores/courseEnrollment";
import { useChapterStore } from "@/stores/chapter";
import { usePageStore } from "@/stores/page";
import LessonPlayerDialog from "@/components/lesson-player/LessonPlayerDialog.vue";

const { t } = useI18n();

const enrollmentStore = useCourseEnrollmentStore();
const { enrollments, loading } = storeToRefs(enrollmentStore);

const chapterStore = useChapterStore();
const { items: chapters, loading: chaptersLoading } =
  storeToRefs(chapterStore);

const pageStore = usePageStore();

const expandedCourse = ref<string>();
const chapterProgress = ref<Record<string, number>>({});
const chapterProgressLoading = ref(false);

onMounted(async () => {
  await enrollmentStore.getMyEnrollments();
});

const loadChapterProgress = async () => {
  chapterProgressLoading.value = true;
  try {
    await Promise.all(
      chapters.value.map(async (chapter) => {
        const result = await pageStore.getChapterPlayer(chapter.id);
        if (result.succeeded && result.data) {
          chapterProgress.value[chapter.id] = result.data.percentComplete || 0;
        }
      }),
    );
  } finally {
    chapterProgressLoading.value = false;
  }
};

const handleCourseExpand = async (courseId: string) => {
  chapterProgress.value = {};
  chapterStore.setFilters({ search: null, courseId });
  await chapterStore.getPaginatedItems();
  await loadChapterProgress();
};

const playerChapterId = ref<string | null>(null);
const playerCategoryTitle = ref<string | null>(null);
const playerOpen = ref(false);

const goToChapter = (chapterId: string, categoryTitle?: string) => {
  playerChapterId.value = chapterId;
  playerCategoryTitle.value = categoryTitle ?? null;
  playerOpen.value = true;
};

const refreshProgress = async () => {
  await enrollmentStore.getMyEnrollments();
  await loadChapterProgress();
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

          <v-list v-else density="compact">
            <v-list-item
              v-for="chapter in chapters"
              :key="chapter.id"
            >
              <template #prepend>
                <v-icon
                  :icon="
                    (chapterProgress[chapter.id] || 0) >= 100
                      ? 'mdi-check-circle'
                      : (chapterProgress[chapter.id] || 0) > 0
                        ? 'mdi-progress-clock'
                        : 'mdi-play-circle-outline'
                  "
                  :color="
                    (chapterProgress[chapter.id] || 0) >= 100
                      ? 'success'
                      : (chapterProgress[chapter.id] || 0) > 0
                        ? 'warning'
                        : 'grey'
                  "
                  size="small"
                />
              </template>
              <v-list-item-title>{{ chapter.title }}</v-list-item-title>
              <v-list-item-subtitle v-if="!chapterProgressLoading">
                {{ chapterProgress[chapter.id] || 0 }}%
                {{ t("learning.myCourses.percentComplete") }}
              </v-list-item-subtitle>

              <template #append>
                <v-btn
                  color="primary"
                  size="small"
                  variant="flat"
                  @click="goToChapter(chapter.id, enrollment.categoryTitle)"
                >
                  {{
                    (chapterProgress[chapter.id] || 0) >= 100
                      ? t("learning.myCourses.review")
                      : (chapterProgress[chapter.id] || 0) > 0
                        ? t("learning.myCourses.continue")
                        : t("learning.myCourses.start")
                  }}
                </v-btn>
              </template>
            </v-list-item>
          </v-list>
        </v-expansion-panel-text>
      </v-expansion-panel>
    </v-expansion-panels>

    <LessonPlayerDialog
      v-model="playerOpen"
      :chapter-id="playerChapterId"
      :category-title="playerCategoryTitle"
    />
  </v-container>
</template>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}
</style>
