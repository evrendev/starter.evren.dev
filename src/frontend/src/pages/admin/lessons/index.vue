<script setup lang="ts">
import { Notify } from "@/stores/notification";

import { DataTable, LessonFilter } from "@/views/admin/lessons";

import { useChapterStore } from "@/stores/chapter";
import { useLessonStore } from "@/stores/lesson";
import { AdvancedFilters, Filters } from "@/types/requests/lesson";

const lessonStore = useLessonStore();
const { loading, total, itemsPerPage, items } = storeToRefs(lessonStore);

const chapterStore = useChapterStore();
const { items: chapters } = storeToRefs(chapterStore);

const { t } = useI18n();
const route = useRoute();

onMounted(async () => {
  await chapterStore.getAllItems();
});

const headers = computed(() => [
  ...([
    {
      title: t("admin.lessons.fields.image.title"),
      key: "image",
      align: "center",
      sortable: false,
      width: "48px",
    },
    {
      title: t("admin.lessons.fields.chapterId.title"),
      key: "chapterTitle",
      sortable: false,
      width: "72px",
    },
    {
      title: t("admin.lessons.fields.title.title"),
      key: "title",
      sortable: true,
      width: "150px",
    },
    {
      title: t("admin.lessons.fields.description.title"),
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
    title: t("admin.components.breadcrumbs.admin.lessons"),
    to: { path: "/admin/lessons" },
  },
  {
    title: t(route.meta.title as string),
    disabled: true,
  },
]);

const handleDelete = async (id: string | null) => {
  if (id) {
    const response = await lessonStore.delete(id);

    if (response.succeeded) {
      Notify.success(t("admin.lessons.notifications.deleted"));
    } else {
      Notify.error(t("admin.lessons.notifications.deleteFailed"));
    }
  }
};

const handleUpdateFilters = async (filters: AdvancedFilters) => {
  lessonStore.setFilters(filters);
  await lessonStore.getPaginatedItems();
};

const handleResetFilters = async () => {
  lessonStore.resetFilters();
  await lessonStore.getPaginatedItems();
};

const getPaginatedItems = async (options: Filters) => {
  lessonStore.setFilters(options);
  await lessonStore.getPaginatedItems();
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <lesson-filter
    :page-title="t('shared.filters.title')"
    :disabled="loading"
    :chapters="chapters"
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
