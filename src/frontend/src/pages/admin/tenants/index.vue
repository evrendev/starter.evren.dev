<script setup lang="ts">
import { DataTable, TenantFilter } from "@/views/admin/tenants";

import { Notify } from "@/stores/notification";
import { useTenantStore } from "@/stores/tenant";
import { BasicFilters, Filters, UpgradeTenant } from "@/types/requests/tenant";

const tenantStore = useTenantStore();
const { loading, total, itemsPerPage, items } = storeToRefs(tenantStore);

const { t } = useI18n();
const route = useRoute();

const headers = computed(() => [
  ...([
    {
      title: t("admin.tenants.fields.id.title"),
      key: "id",
      sortable: true,
    },
    {
      title: t("admin.tenants.fields.isActive.title"),
      key: "isActive",
      align: "center",
      sortable: false,
    },
    {
      title: t("admin.tenants.fields.name.title"),
      key: "name",
      align: "center",
      sortable: false,
    },
    {
      title: t("admin.tenants.fields.adminEmail.title"),
      key: "adminEmail",
      align: "center",
      sortable: false,
    },
    {
      title: t("admin.tenants.fields.validUpto.title"),
      key: "validUpto",
      align: "center",
      sortable: true,
    },
    {
      title: t("shared.actions"),
      key: "actions",
      align: "center",
      sortable: false,
    },
  ] as const),
]);

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { name: "dashboard" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.tenants"),
    to: { path: "/admin/tenants" },
  },
  {
    title: t(route.meta.title as string),
    disabled: true,
  },
]);

const handleDelete = async (id: string | null) => {
  if (id) {
    const response = await tenantStore.delete(id);

    if (response.succeeded) {
      Notify.success(t("admin.tenants.notifications.deleted"));
    } else {
      Notify.error(t("admin.tenants.notifications.deleteFailed"));
    }
  }
};

const handleActivate = async (id: string) => {
  const response = await tenantStore.activate(id);

  if (response.succeeded) {
    Notify.success(t("admin.tenants.notifications.activated"));
  } else {
    Notify.error(t("admin.tenants.notifications.activateFailed"));
  }
};

const handleDeactivate = async (id: string) => {
  const response = await tenantStore.deactivate(id);

  if (response.succeeded) {
    Notify.success(t("admin.tenants.notifications.deactivated"));
  } else {
    Notify.error(t("admin.tenants.notifications.deactivateFailed"));
  }
};

const handleUpgrade = async (tenant: UpgradeTenant) => {
  const response = await tenantStore.upgrade(tenant);

  if (response.succeeded) {
    Notify.success(t("admin.tenants.notifications.upgraded"));
  } else {
    Notify.error(t("admin.tenants.notifications.upgradeFailed"));
  }
};

const handleUpdateFilters = async (filters: BasicFilters) => {
  tenantStore.setFilters(filters);
  await tenantStore.getItems();
};

const handleResetFilters = async () => {
  tenantStore.resetFilters();
  await tenantStore.getItems();
};

const getItems = async (options: Filters) => {
  tenantStore.setFilters(options);
  await tenantStore.getItems();
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <tenant-filter
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
    @activate="handleActivate"
    @deactivate="handleDeactivate"
    @upgrade="handleUpgrade"
    @update:options="getItems"
    item-value="id"
  />
</template>
