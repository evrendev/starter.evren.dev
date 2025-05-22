<script setup>
import { useI18n } from "vue-i18n";
import { onMounted, ref, shallowRef } from "vue";
import { Breadcrumb } from "@/components/forms";
import { useFountainDonationStore, useAppStore } from "@/stores";
import { storeToRefs } from "pinia";
import { StatsCard, DoughnutCard, DataTable, MonthlyProjectBarChart } from "./components/";

const { t } = useI18n();
const page = ref({ title: t("components.sidebar.dashboard.title") });
const breadcrumbs = shallowRef([
  {
    title: t("components.sidebar.dashboard.header"),
    disabled: false,
    href: "#"
  }
]);

const appStore = useAppStore();
const { loading } = storeToRefs(appStore);
const fountainDonationStore = useFountainDonationStore();
const { doughnutChartData, monthlyProjectStats, donations } = storeToRefs(fountainDonationStore);

onMounted(() => {
  fountainDonationStore.getMetrics();
});

const colorMap = {
  primary: "#009846",
  warning: "#ffc107",
  info: "#03c9d7",
  error: "#f44336"
};
</script>

<template>
  <breadcrumb :title="page.title" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col md="12">
      <v-sheet class="pa-2 ma-2">
        <monthly-project-bar-chart
          v-if="monthlyProjectStats"
          :loading="loading"
          :monthly-project-stats="monthlyProjectStats"
          :color-map="colorMap"
        />
      </v-sheet>
    </v-col>
  </v-row>
  <v-row class="mt-2">
    <v-col lg="8" sm="12" md="9">
      <v-sheet class="pa-2 ma-2">
        <data-table :loading="loading" v-if="donations" :items="donations" hide-default-footer />
      </v-sheet>
    </v-col>
    <v-col lg="4" sm="12" md="3">
      <v-sheet class="pa-2 ma-2">
        <doughnut-card :chart-data="doughnutChartData" :color-map="colorMap" />
      </v-sheet>
      <v-sheet class="pa-2 ma-2">
        <stats-card v-for="(item, index) in doughnutChartData" :key="index" :item="item" class="mb-2" />
      </v-sheet>
    </v-col>
  </v-row>
</template>
