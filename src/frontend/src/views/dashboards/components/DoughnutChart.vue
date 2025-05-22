<script setup>
import { ref, computed } from "vue";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";
import { Doughnut } from "vue-chartjs";

ChartJS.register(ArcElement, Tooltip, Legend);

const props = defineProps({
  totalFountainCountsByProject: {
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

const data = computed(() => {
  if (!props.totalFountainCountsByProject?.length) return null;

  return {
    labels: props.totalFountainCountsByProject?.map((s) => s.project.name),
    datasets: [
      {
        data: props.totalFountainCountsByProject?.map((p) => p.count),
        backgroundColor: props.totalFountainCountsByProject?.map((p) => props.colorMap[p.project.color] || "#CCCCCC")
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
  <Doughnut v-if="data" :data="data" :options="options" />
</template>
