<script setup>
import { ref, watch } from "vue";
import { useFountainDonationStore } from "@/stores";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { Breadcrumb } from "@/components/forms";
import { FilterCard, DataTable } from "./components/";
import config from "@/config";

const { t } = useI18n();
const route = useRoute();
const projectCode = ref(route.query.projectCode || null);

const breadcrumbs = ref([
  {
    title: t("admin.donations.title"),
    disabled: true,
    href: "/admin/donations"
  },
  {
    title: t("admin.donations.fountains.title"),
    disabled: true,
    href: "#"
  },
  {
    title: t(`components.sidebar.donations.fountains.${projectCode.value ?? "all"}`),
    href: "#"
  }
]);

watch(
  () => route.query,
  (query) => {
    projectCode.value = query.projectCode;
    breadcrumbs.value = [
      {
        title: t("admin.donations.title"),
        disabled: true,
        href: "/admin/donations"
      },
      {
        title: t("admin.donations.fountains.title"),
        disabled: true,
        href: "#"
      },
      {
        title: t(`components.sidebar.donations.fountains.${projectCode.value ?? "all"}`),
        href: "#"
      }
    ];

    searchOptions.projectCode = projectCode.value;
    getItems(searchOptions);
  }
);

const items = ref([]);
const itemsLength = ref(0);
const loading = ref(false);
const donationStore = useFountainDonationStore();

const searchOptions = {
  page: 1,
  itemsPerPage: config.itemsPerPage,
  sortBy: null,
  groupBy: null,
  projectCode: projectCode.value,
  startDate: null,
  endDate: null,
  search: null
};

const handleFilterSubmit = (filters) => {
  searchOptions.page = 1;
  searchOptions.startDate = filters.startDate;
  searchOptions.endDate = filters.endDate;
  searchOptions.search = filters.search;
  getItems(searchOptions);
};

const handleFilterReset = () => {
  searchOptions.page = 1;
  searchOptions.startDate = null;
  searchOptions.endDate = null;
  searchOptions.search = null;
  getItems(searchOptions);
};

const getItems = async (options) => {
  loading.value = true;
  options.projectCode = projectCode.value;
  await donationStore.getItems(options);
  items.value = donationStore.items;
  itemsLength.value = donationStore.itemsLength;
  loading.value = false;
};
</script>

<template>
  <breadcrumb :title="t('admin.donations.fountains.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <filter-card :loading="loading" @submit="handleFilterSubmit" @reset="handleFilterReset" />

      <data-table :items="items" :items-length="itemsLength" :loading="loading" @update:options="getItems" />
    </v-col>
  </v-row>
</template>
