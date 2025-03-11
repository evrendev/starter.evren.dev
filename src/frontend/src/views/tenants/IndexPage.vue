<script setup>
import { shallowRef } from "vue";
import { useTenantStore } from "@/stores";
import { useI18n } from "vue-i18n";
import { DataTable, FilterCard } from "./components";
import { Breadcrumb } from "@/components/forms";
import { storeToRefs } from "pinia";
import config from "@/config";

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
const { loading, items, itemsLength } = storeToRefs(tenantStore);

const searchOptions = {
  page: 1,
  itemsPerPage: config.itemsPerPage,
  sortBy: null,
  groupBy: null,
  showActiveItems: true,
  showDeletedItems: false,
  startDate: null,
  endDate: null,
  search: null
};

const handleFilterSubmit = async (filters) => {
  searchOptions.page = 1;
  searchOptions.showActiveItems = filters.showActiveItems;
  searchOptions.showDeletedItems = filters.showDeletedItems;
  searchOptions.startDate = filters.startDate;
  searchOptions.endDate = filters.endDate;
  searchOptions.search = filters.search;

  await getItems();
};

const handleFilterReset = async () => {
  searchOptions.page = 1;
  searchOptions.showActiveItems = true;
  searchOptions.showDeletedItems = false;
  searchOptions.startDate = null;
  searchOptions.endDate = null;
  searchOptions.search = null;

  await getItems();
};

const getItems = async () => {
  await tenantStore.getItems(searchOptions);
};
</script>

<template>
  <breadcrumb :title="t('admin.tenants.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <filter-card :loading="loading" @submit="handleFilterSubmit" @reset="handleFilterReset" />

      <data-table :items="items" :items-length="itemsLength" :loading="loading" @update:options="getItems" />
    </v-col>
  </v-row>
</template>
