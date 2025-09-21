<script setup lang="ts">
import { DataTable, UserFilter } from "@/views/admin/users";

import { Notify } from "@/stores/notification";
import { useUserStore } from "@/stores/user";
import { BasicFilters, Filters } from "@/types/requests/user";

const userStore = useUserStore();
const { loading, total, itemsPerPage, items } = storeToRefs(userStore);

const { t } = useI18n();
const route = useRoute();

const headers = computed(() => [
  ...([
    {
      title: t("admin.users.fields.initial.title"),
      key: "initial",
      align: "center",
      sortable: false,
      width: "64px",
    },
    {
      title: t("admin.users.fields.isActive.title"),
      key: "isActive",
      align: "center",
      sortable: false,
      width: "64px",
    },
    {
      title: t("admin.users.fields.fullName.title"),
      key: "fullName",
      sortable: false,
    },
    {
      title: t("admin.users.fields.email.title"),
      key: "email",
      sortable: false,
    },
    {
      title: t("admin.users.fields.twoFactorEnabled.title"),
      key: "twoFactorEnabled",
      align: "center",
      sortable: false,
      width: "64px",
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
    title: t("admin.components.breadcrumbs.admin.users"),
    to: { path: "/admin/users" },
  },
  {
    title: t(route.meta.title as string),
    disabled: true,
  },
]);

const handleDelete = async (id: string | null) => {
  if (id) {
    const response = await userStore.delete(id);

    if (response.succeeded) {
      Notify.success(t("admin.users.notifications.deleted"));
    } else {
      Notify.error(t("admin.users.notifications.deleteFailed"));
    }
  }
};

const handleUpdateFilters = async (filters: BasicFilters) => {
  userStore.setFilters(filters);
  await userStore.getPaginatedItems();
};

const handleResetFilters = async () => {
  userStore.resetFilters();
  await userStore.getPaginatedItems();
};

const getPaginatedItems = async (options: Filters) => {
  userStore.setFilters(options);
  await userStore.getPaginatedItems();
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <user-filter
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
