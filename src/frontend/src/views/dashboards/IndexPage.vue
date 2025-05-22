<script setup>
import { useI18n } from "vue-i18n";
import { onMounted, ref, shallowRef } from "vue";
import { Breadcrumb } from "@/components/forms";
import { useFountainDonationStore, useAppStore } from "@/stores";
import { storeToRefs } from "pinia";
import { StatsCard, DataTable, DoughnutChart, MonthlyProjectStatsChart } from "./components/";

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
const { totalFountainCountsByProject, monthlyProjectStats, recentFountainDonations } = storeToRefs(fountainDonationStore);

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
        <monthly-project-stats-chart
          v-if="monthlyProjectStats"
          :monthly-project-stats="monthlyProjectStats"
          :loading="loading"
          :color-map="colorMap"
        />
      </v-sheet>
    </v-col>
  </v-row>
  <v-row class="mt-2">
    <v-col lg="8" sm="12" md="9">
      <v-sheet class="pa-2 ma-2">
        <data-table
          v-if="recentFountainDonations"
          :recent-fountain-donations="recentFountainDonations"
          :loading="loading"
          hide-default-footer
        />
      </v-sheet>
    </v-col>
    <v-col lg="4" sm="12" md="3">
      <v-sheet class="pa-2 ma-2">
        <doughnut-chart
          v-if="totalFountainCountsByProject"
          :total-fountain-counts-by-project="totalFountainCountsByProject"
          :color-map="colorMap"
        />
      </v-sheet>
      <v-sheet class="pa-2 ma-2" v-if="totalFountainCountsByProject">
        <stats-card v-for="(item, index) in totalFountainCountsByProject" :key="index" :item="item" class="mb-2" />
      </v-sheet>
    </v-col>
  </v-row>
</template>
