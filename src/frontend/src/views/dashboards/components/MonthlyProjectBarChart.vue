<script setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import VueApexCharts from "vue3-apexcharts";

const { t } = useI18n();

const props = defineProps({
  monthlyProjectStats: {
    type: Object,
    default: () => ({}),
    required: false
  }
});

const monthOrder = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

const rawCategories = computed(() => {
  const months = [...new Set(props.monthlyProjectStats.map((s) => s.month))];
  return monthOrder.filter((m) => months.includes(m));
});

const translatedCategories = computed(() => rawCategories.value.map((m) => t(`common.months.${m}`)));

const projects = computed(() => {
  return [...new Set(props.monthlyProjectStats.map((s) => s.project))];
});

const chartSeries = computed(() => {
  return projects.value.map((project) => {
    return {
      name: project,
      data: rawCategories.value.map((month) => {
        const record = props.monthlyProjectStats.find((s) => s.project === project && s.month === month);
        return record ? record.count : 0;
      })
    };
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
    categories: translatedCategories.value // ✔️ Lokalize edilmiş ay adları burada
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
  }
}));
</script>

<template>
  <vue-apex-charts width="100%" height="400" type="bar" :options="chartOptions" :series="chartSeries" />
</template>
