<script setup lang="ts">
import { useCourseStore } from "@/stores/course";
import { useCourseEnrollmentStore } from "@/stores/courseEnrollment";
import { Notify } from "@/stores/notification";

const { t } = useI18n();
const router = useRouter();

const courseStore = useCourseStore();
const { items: courses, loading } = storeToRefs(courseStore);

const enrollmentStore = useCourseEnrollmentStore();
const { enrollments, loading: enrollmentLoading } =
  storeToRefs(enrollmentStore);

const enrollingCourseId = ref<string | null>(null);

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

const goToMyCourses = () => {
  router.push({ name: "learning-my-courses" });
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
              @click="goToMyCourses"
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
  </v-container>
</template>
