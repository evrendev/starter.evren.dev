<script setup lang="ts">
import breadcrumb from "@/components/admin/breadcrumb.vue";
import DataTable from "@/views/admin/tenants/DataTable.vue";

import { useTenantStore } from "@/stores/tenant";
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
</script>

<template>
  <breadcrumb :items="breadcrumbs" />
  <data-table
    :items-per-page="itemsPerPage"
    :items="items"
    :total="total"
    :loading="loading"
    :headers="headers"
  />
</template>
