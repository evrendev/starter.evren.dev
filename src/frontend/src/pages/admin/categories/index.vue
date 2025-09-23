<script setup lang="ts">
import { DataTable, CategoryFilter } from "@/views/admin/categories";

import { Notify } from "@/stores/notification";
import { useCategoryStore } from "@/stores/category";
import { AdvancedFilters, Filters } from "@/types/requests/category";

const categoryStore = useCategoryStore();
const { loading, total, itemsPerPage, items } = storeToRefs(categoryStore);

const { t } = useI18n();
const route = useRoute();

const headers = computed(() => [
  ...([
    {
      title: t("admin.categories.fields.title.title"),
      key: "title",
      align: "center",
      sortable: false,
      width: "150px",
    },
    {
      title: t("admin.categories.fields.description.title"),
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
    title: t("admin.components.breadcrumbs.admin.categories"),
    to: { path: "/admin/categories" },
  },
  {
    title: t(route.meta.title as string),
    disabled: true,
  },
]);

const handleDelete = async (id: string | null) => {
  if (id) {
    const response = await categoryStore.delete(id);

    if (response.succeeded) {
      Notify.success(t("admin.categories.notifications.deleted"));
    } else {
      Notify.error(t("admin.categories.notifications.deleteFailed"));
    }
  }
};

const handleUpdateFilters = async (filters: AdvancedFilters) => {
  categoryStore.setFilters(filters);
  await categoryStore.getPaginatedItems();
};

const handleResetFilters = async () => {
  categoryStore.resetFilters();
  await categoryStore.getPaginatedItems();
};

const getPaginatedItems = async (options: Filters) => {
  categoryStore.setFilters(options);
  await categoryStore.getPaginatedItems();
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <category-filter
    :page-title="t('shared.filters.title')"
    :disabled="loading"
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
