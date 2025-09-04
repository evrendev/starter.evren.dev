<script setup lang="ts">
import DataTable from "@/views/admin/tenants/DataTable.vue";

import { Notify } from "@/stores/notification";
import { useTenantStore } from "@/stores/tenant";
import { DefaultApiResponse } from "@/responses/api";
import { AxiosResponse } from "axios";
const tenantStore = useTenantStore();
const filters = tenantStore.filters;
const { itemsPerPage, items, total, loading } = storeToRefs(tenantStore);

const { t } = useI18n();
const route = useRoute();

onMounted(async () => {
  await tenantStore.getItems(filters);
});

const headers = computed(() => [
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

    if (response) {
      Notify.success(t("admin.tenants.notifications.deleted"));
    } else {
      Notify.error(t("admin.tenants.notifications.deleteFailed"));
    }
  }
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <data-table
    :items-per-page="itemsPerPage"
    :items="items"
    :total="total"
    :loading="loading"
    :headers="headers"
    @delete="handleDelete"
  />
</template>
