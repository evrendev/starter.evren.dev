<script setup>
import { ref, shallowRef } from "vue";
import { useAuditStore } from "@/stores";
import { useI18n } from "vue-i18n";
import { Breadcrumb } from "@/components/forms";
import { FilterCard, DataTable } from "./components/";
import config from "@/config";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.audits.title"),
    disabled: true,
    href: "#"
  }
]);

const items = ref([]);
const itemsLength = ref(0);
const loading = ref(false);
const auditStore = useAuditStore();

const searchOptions = {
  page: 1,
  itemsPerPage: config.itemsPerPage,
  sortBy: null,
  groupBy: null,
  action: null,
  startDate: null,
  endDate: null,
  search: null
};

const handleFilterSubmit = (filters) => {
  searchOptions.page = 1;
  searchOptions.action = filters.action;
  searchOptions.startDate = filters.startDate;
  searchOptions.endDate = filters.endDate;
  searchOptions.search = filters.search;
  getItems(searchOptions);
};

const handleFilterReset = () => {
  searchOptions.page = 1;
  searchOptions.action = null;
  searchOptions.startDate = null;
  searchOptions.endDate = null;
  searchOptions.search = null;
  getItems(searchOptions);
};

const getItems = async (options) => {
  loading.value = true;
  await auditStore.getItems(options);
  items.value = auditStore.items;
  itemsLength.value = auditStore.itemsLength;
  loading.value = false;
};
</script>

<template>
  <breadcrumb :title="t('admin.audits.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <filter-card :loading="loading" @submit="handleFilterSubmit" @reset="handleFilterReset" />

      <data-table :items="items" :items-length="itemsLength" :loading="loading" @update:options="getItems" />
    </v-col>
  </v-row>
</template>
