<script setup lang="ts">
import { Notify } from "@/stores/notification";
import { CourseForm } from "@/views/admin/courses";

import { useCategoryStore } from "@/stores/category";
import { useCourseStore } from "@/stores/course";
import { Course } from "@/models/course";
import { Result } from "@/primitives/result";
const { t } = useI18n();

const categoryStore = useCategoryStore();
const { items: categories } = storeToRefs(categoryStore);

const courseStore = useCourseStore();
const { loading, course } = storeToRefs(courseStore);

const route = useRoute();
const router = useRouter();

watch(
  () => route.params.id,
  async (id) => {
    if (id) {
      await categoryStore.getAllItems();
      await courseStore.getById(id as string);
    } else {
      courseStore.$reset();
    }
  },
  {
    immediate: true,
  },
);

const pageTitle: ComputedRef<string> = computed(() =>
  t(route.meta.title as string),
);

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { path: "/admin" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.courses"),
    to: { path: "/admin/courses" },
  },
  {
    title: pageTitle.value,
    disabled: true,
  },
]);

const handleSubmit = async (values: Course) => {
  const response: Result<Course> =
    route.name === "course-create"
      ? await courseStore.create(values)
      : await courseStore.update(values);

  if (response.succeeded) {
    Notify.success(
      t(
        `admin.courses.notifications.${route.name === "course-create" ? "created" : "updated"}`,
      ),
    );
    router.push({ name: "course-list" });
  } else {
    Notify.error(
      t(
        `admin.courses.notifications.${route.name === "course-create" ? "createFailed" : "updateFailed"}`,
      ),
    );
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <course-form
    :course="course"
    :categories="categories"
    :loading="loading"
    :route-name="route.name"
    :page-title="pageTitle"
    @submit="handleSubmit"
  />
</template>
