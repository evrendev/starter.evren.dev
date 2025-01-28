<script setup>
import { ref, shallowRef } from "vue";
import { useTenantStore } from "@/stores";
import { useI18n } from "vue-i18n";
import BaseBreadcrumb from "@/components/shared/BaseBreadcrumb.vue";
import { DataTable, FilterCard } from "./components";
import config from "@/config";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.tenants.title"),
    disabled: true,
    href: "#"
  }
]);

const items = ref([]);
const itemsLength = ref(0);
const loading = ref(false);
const tenantStore = useTenantStore();

const searchOptions = {
  page: 1,
  itemsPerPage: config.itemsPerPage,
  sortBy: null,
  groupBy: null,
  isActive: null,
  startDate: null,
  endDate: null,
  search: null
};

const handleFilterSubmit = (filters) => {
  console.log("filters", filters);
  searchOptions.page = 1;
  searchOptions.isActive = filters.isActive;
  searchOptions.startDate = filters.startDate;
  searchOptions.endDate = filters.endDate;
  searchOptions.search = filters.search;
  console.log("searchOptions", searchOptions);
  getItems(searchOptions);
};

const handleFilterReset = () => {
  searchOptions.page = 1;
  searchOptions.isActive = null;
  searchOptions.startDate = null;
  searchOptions.endDate = null;
  searchOptions.search = null;
  getItems(searchOptions);
};

const getItems = async (options) => {
  loading.value = true;
  await tenantStore.getItems(options);
  items.value = tenantStore.items;
  itemsLength.value = tenantStore.itemsLength;
  loading.value = false;
};
</script>

<template>
  <base-breadcrumb :title="t('admin.tenants.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <filter-card :loading="loading" @submit="handleFilterSubmit" @reset="handleFilterReset" />

      <data-table :items="items" :items-length="itemsLength" :loading="loading" @update:options="getItems" />
    </v-col>
  </v-row>
</template>
