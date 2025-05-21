<script setup>
import { useI18n } from "vue-i18n";
import { computed, onMounted, ref, shallowRef } from "vue";
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
const { overview } = storeToRefs(fountainDonationStore);

onMounted(() => {
  fountainDonationStore.getOverviews();
});

const colorMap = {
  primary: "#009846",
  warning: "#ffc107",
  info: "#03c9d7",
  error: "#f44336"
};

const doughnutChartData = computed(() => {
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

const monthOrder = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

const rawCategories = computed(() => {
  const months = [...new Set(overview.value.monthlyProjectStats.map((s) => s.month))];
  return monthOrder.filter((m) => months.includes(m));
});

const translatedCategories = computed(() => rawCategories.value.map((m) => t(`common.months.${m}`)));

const projects = computed(() => {
  return [...new Set(overview.value.monthlyProjectStats.map((s) => s.project.name))];
});

const chartSeries = computed(() => {
  return projects.value.map((project) => {
    return {
      name: project,
      data: rawCategories.value.map((month) => {
        const record = overview.value.monthlyProjectStats.find((s) => s.project.name === project && s.month === month);
        return record ? record.count : 0;
      })
    };
  });
});

const chartColors = computed(() => {
  return projects.value.map((project) => {
    console.log(project);
    const record = overview.value.monthlyProjectStats.find((s) => s.project.name === project);
    return record ? colorMap[record.project.color] || "#CCCCCC" : "#CCCCCC";
  });
});

const chartOptions = computed(() => ({
  chart: {
    type: "bar",
    height: 400,
    stacked: true
  },
  plotOptions: {
    bar: {
      horizontal: false,
      columnWidth: "55%",
      borderRadius: 5
    }
  },
  dataLabels: {
    enabled: false
  },
  stroke: {
    show: true,
    width: 1,
    colors: ["transparent"]
  },
  xaxis: {
    categories: translatedCategories.value
  },
  yaxis: {
    title: {
      text: t("admin.dashboard.chart.yaxis")
    }
  },
  fill: {
    opacity: 1
  },
  tooltip: {
    y: {
      formatter: (val) => `${val} ${t("common.fountains")}`
    }
  },
  colors: chartColors.value
}));
</script>

<template>
  <breadcrumb :title="page.title" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col md="12">
      <v-sheet class="pa-2 ma-2">
        <monthly-project-bar-chart
          v-if="overview?.donations"
          :loading="loading"
          :chart-series="chartSeries"
          :chart-options="chartOptions"
        />
      </v-sheet>
    </v-col>
  </v-row>
  <v-row class="mt-2">
    <v-col lg="8" sm="12" md="9">
      <v-sheet class="pa-2 ma-2">
        <data-table :loading="loading" v-if="overview?.donations" :items="overview?.donations" hide-default-footer />
      </v-sheet>
    </v-col>
    <v-col lg="4" sm="12" md="3">
      <v-sheet class="pa-2 ma-2">
        <doughnut-card :chart-data="doughnutChartData" />
      </v-sheet>
      <v-sheet class="pa-2 ma-2">
        <stats-card v-for="(item, index) in overview?.stats" :key="index" :item="item" class="mb-2" />
      </v-sheet>
    </v-col>
  </v-row>
</template>
