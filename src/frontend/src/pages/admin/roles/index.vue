<script setup lang="ts">
import { DataTable, RoleFilter } from "@/views/admin/roles";

import { Notify } from "@/stores/notification";
import { useRoleStore } from "@/stores/role";
import { AdvancedFilters, Filters } from "@/types/requests/role";

const roleStore = useRoleStore();
const { loading, total, itemsPerPage, items } = storeToRefs(roleStore);

const { t } = useI18n();
const route = useRoute();

const headers = computed(() => [
  ...([
    {
      title: t("admin.roles.fields.name.title"),
      key: "name",
      align: "center",
      sortable: false,
      width: "150px",
    },
    {
      title: t("admin.roles.fields.description.title"),
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
    title: t("admin.components.breadcrumbs.admin.roles"),
    to: { path: "/admin/roles" },
  },
  {
    title: t(route.meta.title as string),
    disabled: true,
  },
]);

const handleDelete = async (id: string | null) => {
  if (id) {
    const response = await roleStore.delete(id);

    if (response.succeeded) {
      Notify.success(t("admin.roles.notifications.deleted"));
    } else {
      Notify.error(t("admin.roles.notifications.deleteFailed"));
    }
  }
};

const handleUpdateFilters = async (filters: AdvancedFilters) => {
  roleStore.setFilters(filters);
  await roleStore.getPaginatedItems();
};

const handleResetFilters = async () => {
  roleStore.resetFilters();
  await roleStore.getPaginatedItems();
};

const getPaginatedItems = async (options: Filters) => {
  roleStore.setFilters(options);
  await roleStore.getPaginatedItems();
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <role-filter
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
