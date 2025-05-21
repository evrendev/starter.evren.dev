<script setup>
import { ref } from "vue";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";
import { Doughnut } from "vue-chartjs";

ChartJS.register(ArcElement, Tooltip, Legend);

defineProps({
  chartData: {
    type: Object,
    default: () => ({}),
    required: false
  }
});

const options = ref({
  responsive: true,
  maintainAspectRatio: false
});
</script>

<template>
  <Doughnut v-if="chartData" :data="chartData" :options="options" />
</template>
