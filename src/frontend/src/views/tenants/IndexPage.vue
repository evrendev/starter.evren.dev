<script setup>
import { shallowRef } from "vue";
import { useTenantStore } from "@/stores";
import { useI18n } from "vue-i18n";
import { DataTable, FilterCard } from "./components";
import { Breadcrumb } from "@/components/forms";
import config from "@/config";
import { storeToRefs } from "pinia";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.tenants.title"),
    disabled: true,
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
  isActive: null,
  showDeletedItems: false,
  startDate: null,
  endDate: null,
  search: null
};

const handleFilterSubmit = async (filters) => {
  searchOptions.page = 1;
  searchOptions.isActive = filters.isActive;
  searchOptions.showDeletedItems = filters.showDeletedItems;
  searchOptions.startDate = filters.startDate;
  searchOptions.endDate = filters.endDate;
  searchOptions.search = filters.search;

  await getItems(searchOptions);
};

const handleFilterReset = async () => {
  searchOptions.page = 1;
  searchOptions.isActive = null;
  searchOptions.showDeletedItems = false;
  searchOptions.startDate = null;
  searchOptions.endDate = null;
  searchOptions.search = null;

  await getItems(searchOptions);
};

const getItems = async (options) => {
  await tenantStore.getItems(options);
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
