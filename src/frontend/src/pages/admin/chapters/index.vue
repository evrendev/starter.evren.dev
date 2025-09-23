<script setup lang="ts">
import { Notify } from "@/stores/notification";

import { DataTable, ChapterFilter } from "@/views/admin/chapters";

import { useCourseStore } from "@/stores/course";
import { useChapterStore } from "@/stores/chapter";
import { BasicFilters, Filters } from "@/types/requests/chapter";

const chapterStore = useChapterStore();
const { loading, total, itemsPerPage, items } = storeToRefs(chapterStore);

const courseStore = useCourseStore();
const { items: courses } = storeToRefs(courseStore);

const { t } = useI18n();
const route = useRoute();

onMounted(async () => {
  await courseStore.getAllItems();
});

const headers = computed(() => [
  ...([
    {
      title: t("admin.chapters.fields.courseId.title"),
      key: "courseTitle",
      sortable: false,
      width: "100px",
    },
    {
      title: t("admin.chapters.fields.title.title"),
      key: "title",
      sortable: true,
      width: "100px",
    },
    {
      title: t("admin.chapters.fields.description.title"),
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
    title: t("admin.components.breadcrumbs.admin.chapters"),
    to: { path: "/admin/chapters" },
  },
  {
    title: t(route.meta.title as string),
    disabled: true,
  },
]);

const handleDelete = async (id: string | null) => {
  if (id) {
    const response = await chapterStore.delete(id);

    if (response.succeeded) {
      Notify.success(t("admin.chapters.notifications.deleted"));
    } else {
      Notify.error(t("admin.chapters.notifications.deleteFailed"));
    }
  }
};

const handleUpdateFilters = async (filters: BasicFilters) => {
  chapterStore.setFilters(filters);
  await chapterStore.getPaginatedItems();
};

const handleResetFilters = async () => {
  chapterStore.resetFilters();
  await chapterStore.getPaginatedItems();
};

const getPaginatedItems = async (options: Filters) => {
  chapterStore.setFilters(options);
  await chapterStore.getPaginatedItems();
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <chapter-filter
    :page-title="t('shared.filters.title')"
    :disabled="loading"
    :courses="courses"
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
