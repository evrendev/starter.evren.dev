<script setup>
import { ref, computed } from "vue";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";
import { Doughnut } from "vue-chartjs";

ChartJS.register(ArcElement, Tooltip, Legend);

const props = defineProps({
  chartData: {
    type: Object,
    default: () => ({}),
    required: false
  },
  colorMap: {
    type: Object,
    default: () => ({}),
    required: false
  }
});

const chartData = computed(() => {
  if (!props.chartData?.length) return null;

  return {
    labels: props.chartData?.map((s) => s.project.name),
    datasets: [
      {
        data: props.chartData.map((p) => p.count),
        backgroundColor: props.chartData.map((p) => props.colorMap[p.project.color] || "#CCCCCC")
      }
    ]
  };
});

const options = ref({
  responsive: true,
  maintainAspectRatio: false
});
</script>

<template>
  <Doughnut v-if="chartData" :data="chartData" :options="options" />
</template>
