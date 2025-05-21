<script setup>
import { ref, watch, onMounted } from "vue";
import { storeToRefs } from "pinia";
import { useFountainDonationStore, usePredefinedValuesStore } from "@/stores";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { Breadcrumb } from "@/components/forms";
import { FilterCard, DataTable } from "./components/";
import config from "@/config";

const { t } = useI18n();
const route = useRoute();
const project = ref(route.query.project || null);

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
    title: t(`components.sidebar.donations.fountains.${project.value ?? "all"}`),
    href: "#"
  }
]);

const preDefinedValuesStore = usePredefinedValuesStore();
const { mediaStatuses, fountainTeams } = storeToRefs(preDefinedValuesStore);

onMounted(async () => {
  await preDefinedValuesStore.getMediaStatuses();
  await preDefinedValuesStore.getFountainTeams();
});

watch(
  () => route.query,
  (query) => {
    project.value = query.project;
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
        title: t(`components.sidebar.donations.fountains.${project.value ?? "all"}`),
        href: "#"
      }
    ];

    searchOptions.project = project.value;
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
  project: project.value,
  startDate: null,
  endDate: null,
  search: null,
  mediaStatus: null
};

const handleFilterSubmit = (filters) => {
  searchOptions.page = 1;
  searchOptions.startDate = filters.startDate;
  searchOptions.endDate = filters.endDate;
  searchOptions.search = filters.search;
  searchOptions.mediaStatus = filters.mediaStatus;
  getItems(searchOptions);
};

const handleFilterReset = () => {
  searchOptions.page = 1;
  searchOptions.startDate = null;
  searchOptions.endDate = null;
  searchOptions.search = null;
  searchOptions.mediaStatus = null;
  getItems(searchOptions);
};

const getLastDonations = async () => {
  loading.value = true;
  const message = t("common.checkingForNewDonations");
  await donationStore.getLastDonations(message);
  loading.value = false;
};

const getItems = async (options) => {
  loading.value = true;
  options.project = project.value;
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
      <filter-card
        :loading="loading"
        :media-statuses="mediaStatuses"
        @submit="handleFilterSubmit"
        @reset="handleFilterReset"
        @getLastDonations="getLastDonations"
      />

      <data-table
        :items="items"
        :media-statuses="mediaStatuses"
        :fountain-teams="fountainTeams"
        :items-length="itemsLength"
        :loading="loading"
        @update:options="getItems"
        :project="project"
      />
    </v-col>
  </v-row>
</template>
