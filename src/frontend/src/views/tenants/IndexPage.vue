<script setup>
import { shallowRef } from "vue";
import { useTenantStore } from "@/stores";
import { useI18n } from "vue-i18n";
import { DataTable, FilterCard } from "./components";
import { Breadcrumb } from "@/components/forms";
import { storeToRefs } from "pinia";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.tenants.title"),
    disabled: true,
    href: "#"
  },
  {
    title: t("admin.tenants.list"),
    disabled: false,
    href: "#"
  }
]);

const tenantStore = useTenantStore();
const { loading, items, itemsPerPage, page } = storeToRefs(tenantStore);

const handleFilterSubmit = async () => {
  await getItems();
};

const handleFilterReset = async () => {
  tenantStore.resetFilters();
  await getItems();
};

const getItems = async () => {
  await tenantStore.getItems();
};
</script>

<template>
  <breadcrumb :title="t('admin.tenants.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <filter-card :loading="loading" @submit="handleFilterSubmit" @reset="handleFilterReset" />

      <data-table :items="items" :loading="loading" :items-per-page="itemsPerPage" :page="page" @update:options="getItems" />
    </v-col>
  </v-row>
</template>
