<script setup>
import { useI18n } from "vue-i18n";
import { computed, onMounted, ref, shallowRef } from "vue";
import { Breadcrumb } from "@/components/forms";
import { useFountainDonationStore, useAppStore } from "@/stores";
import { storeToRefs } from "pinia";
import { StatsCard, DoughnutCard, DataTable } from "./components/";

const { t } = useI18n();

const appStore = useAppStore();
const { loading } = storeToRefs(appStore);
const fountainDonationStore = useFountainDonationStore();
const { overview } = storeToRefs(fountainDonationStore);

onMounted(() => {
  fountainDonationStore.getOverviews();
});

const options = ref({
  responsive: true,
  maintainAspectRatio: false
});

const colorMap = {
  primary: "#009846",
  warning: "#ffc107",
  info: "#03c9d7",
  error: "#f44336"
};

const chartData = computed(() => {
  if (!overview.value.stats?.length) return null;

  return {
    labels: overview.value.stats?.map((s) => s.project.name),
    datasets: [
      {
        data: overview.value.stats.map((p) => p.count),
        backgroundColor: overview.value.stats.map((p) => colorMap[p.project.color] || "#CCCCCC")
      }
    ]
  };
});

const page = ref({ title: t("components.sidebar.dashboard.title") });
const breadcrumbs = shallowRef([
  {
    title: t("components.sidebar.dashboard.header"),
    disabled: false,
    href: "#"
  }
]);
</script>

<template>
  <breadcrumb :title="page.title" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col lg="8" sm="12" md="9">
      <v-sheet class="pa-2 ma-2">
        <data-table v-if="overview?.donations" :items="overview?.donations" :loading="loading" hide-default-footer />
      </v-sheet>
    </v-col>
    <v-col lg="4" sm="12" md="3">
      <v-sheet class="pa-2 ma-2">
        <doughnut-card :chart-data="chartData" :options="options" />
      </v-sheet>
      <v-sheet class="pa-2 ma-2">
        <stats-card v-for="(item, index) in overview.stats" :key="index" :item="item" class="mb-2" />
      </v-sheet>
    </v-col>
  </v-row>
</template>
