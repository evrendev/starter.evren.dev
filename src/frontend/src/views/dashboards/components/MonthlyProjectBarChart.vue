<script setup>
import VueApexCharts from "vue3-apexcharts";

defineProps({
  chartOptions: {
    type: Object,
    default: () => ({}),
    required: false
  },
  chartSeries: {
    type: Array,
    default: () => [],
    required: false
  }
});
</script>

<template>
  <vue-apex-charts width="100%" height="400" type="bar" :options="chartOptions" :series="chartSeries" />
</template>
