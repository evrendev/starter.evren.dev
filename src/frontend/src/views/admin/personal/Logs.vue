<script lang="ts" setup>
import { DataTable, LogFilter } from "@/views/admin/personal/components";

import { usePersonalStore } from "@/stores/personal";
import { AdvancedFilters, Filters } from "@/types/requests/personal";
const personalStore = usePersonalStore();
const { loading, total, itemsPerPage, items } = storeToRefs(personalStore);

const { t } = useI18n();

const headers = computed(() => [
  ...([
    {
      title: t("admin.personal.logs.fields.type.title"),
      key: "type",
      align: "center",
      sortable: false,
    },
    {
      title: t("admin.personal.logs.fields.tableName.title"),
      key: "tableName",
      align: "center",
      sortable: true,
    },
    {
      title: t("admin.personal.logs.fields.dateTime.title"),
      key: "dateTime",
      align: "center",
      sortable: true,
    },
    {
      title: t("admin.personal.logs.fields.action.title"),
      key: "action",
      align: "center",
      sortable: false,
      width: "100px",
    },
  ] as const),
]);

const handleUpdateFilters = async (filters: AdvancedFilters) => {
  personalStore.setFilters(filters);
  await personalStore.getLogs();
};

const handleResetFilters = async () => {
  personalStore.resetFilters();
  await personalStore.getLogs();
};

const getPaginatedItems = async (options: Filters) => {
  personalStore.setFilters(options);
  await personalStore.getLogs();
};
</script>

<template>
  <log-filter
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
    @update:options="getPaginatedItems"
    item-value="id"
  />
</template>
