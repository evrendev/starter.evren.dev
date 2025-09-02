<script setup lang="ts">
import breadcrumb from "@/components/admin/breadcrumb.vue";
import DataTable from "@/views/admin/tenants/DataTable.vue";
import { useTenantStore } from "@/stores/tenant";
const { t } = useI18n();
const tenantStore = useTenantStore();
const filters = tenantStore.filters;

const { itemsPerPage, items, total, loading } = storeToRefs(tenantStore);

onMounted(async () => {
  await tenantStore.getItems(filters);
});

const headers = shallowRef([
  {
    title: t("admin.tenants.fields.id"),
    key: "id",
    sortable: true,
  },
  {
    title: t("admin.tenants.fields.isActive"),
    key: "isActive",
    align: "center",
    sortable: false,
  },
  {
    title: t("admin.tenants.fields.name"),
    key: "name",
    align: "center",
    sortable: false,
  },
  {
    title: t("admin.tenants.fields.validUpto"),
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

const breadcrumbs = shallowRef([
  {
    title: t("admin.components.breadcrumbs.home"),
    to: { path: "/" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    disabled: true,
  },
  {
    title: t("admin.components.breadcrumbs.admin.tenants"),
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
