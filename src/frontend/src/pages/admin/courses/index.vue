<script setup lang="ts">
import { Notify } from "@/stores/notification";

import { DataTable, CourseFilter } from "@/views/admin/courses";

import { useCategoryStore } from "@/stores/category";
import { useCourseStore } from "@/stores/course";
import { AdvancedFilters, Filters } from "@/types/requests/course";

const courseStore = useCourseStore();
const { loading, total, itemsPerPage, items } = storeToRefs(courseStore);

const categoryStore = useCategoryStore();
const { items: categories } = storeToRefs(categoryStore);

const { t } = useI18n();
const route = useRoute();

onMounted(async () => {
  await categoryStore.getAllItems();
});

const headers = computed(() => [
  ...([
    {
      title: t("admin.courses.fields.image.title"),
      key: "image",
      align: "center",
      sortable: false,
      width: "48px",
    },
    {
      title: t("admin.courses.fields.categoryId.title"),
      key: "categoryTitle",
      sortable: false,
      width: "72px",
    },
    {
      title: t("admin.courses.fields.published.title"),
      key: "published",
      align: "center",
      sortable: true,
      width: "36px",
    },
    {
      title: t("admin.courses.fields.title.title"),
      key: "title",
      sortable: true,
      width: "150px",
    },
    {
      title: t("admin.courses.fields.description.title"),
      key: "description",
      sortable: false,
    },
    {
      title: t("shared.actions"),
      key: "actions",
      align: "center",
      sortable: false,
      width: "100px",
    },
  ] as const),
]);

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { name: "dashboard" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.courses"),
    to: { path: "/admin/courses" },
  },
  {
    title: t(route.meta.title as string),
    disabled: true,
  },
]);

const handleDelete = async (id: string | null) => {
  if (id) {
    const response = await courseStore.delete(id);

    if (response.succeeded) {
      Notify.success(t("admin.courses.notifications.deleted"));
    } else {
      Notify.error(t("admin.courses.notifications.deleteFailed"));
    }
  }
};

const handleUpdateFilters = async (filters: AdvancedFilters) => {
  courseStore.setFilters(filters);
  await courseStore.getPaginatedItems();
};

const handleResetFilters = async () => {
  courseStore.resetFilters();
  await courseStore.getPaginatedItems();
};

const getPaginatedItems = async (options: Filters) => {
  courseStore.setFilters(options);
  await courseStore.getPaginatedItems();
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <course-filter
    :page-title="t('shared.filters.title')"
    :disabled="loading"
    :categories="categories"
    @submit="handleUpdateFilters"
    @reset="handleResetFilters"
  />
  <data-table
    :items-per-page="itemsPerPage"
    :items="items"
    :total="total"
    :loading="loading"
    :headers="headers"
    @delete="handleDelete"
    @update:options="getPaginatedItems"
    item-value="id"
  />
</template>
