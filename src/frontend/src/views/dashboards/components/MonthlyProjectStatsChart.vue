<script setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import VueApexCharts from "vue3-apexcharts";

const { t } = useI18n();
const props = defineProps({
  monthlyProjectStats: {
    type: Array,
    default: () => [],
    required: false
  },
  colorMap: {
    type: Object,
    default: () => ({}),
    required: false
  }
});

const monthOrder = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

const months = computed(() => {
  const months = [...new Set(props.monthlyProjectStats.map((s) => s.month))];
  return monthOrder.filter((m) => months.includes(m));
});

const translatedMonths = computed(() => months.value.map((month) => t(`common.months.${month.toLocaleLowerCase()}.short`)));

const projects = computed(() => {
  return [...new Set(props.monthlyProjectStats.map((s) => s.project.name))];
});

const chartSeries = computed(() => {
  return projects.value.map((project) => {
    return {
      name: project,
      data: months.value.map((month) => {
        const record = props.monthlyProjectStats.find((s) => s.project.name === project && s.month === month);
        return record ? record.count : 0;
      })
    };
  });
});

const chartColors = computed(() => {
  return projects.value.map((project) => {
    const record = props.monthlyProjectStats.find((s) => s.project.name === project);
    return record ? props.colorMap[record.project.color] || "#CCCCCC" : "#CCCCCC";
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
    categories: translatedMonths.value
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
  <vue-apex-charts width="100%" height="400" type="bar" :options="chartOptions" :series="chartSeries" />
</template>
