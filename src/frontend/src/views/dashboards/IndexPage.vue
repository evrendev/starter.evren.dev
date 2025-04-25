<script setup>
import { useI18n } from "vue-i18n";
import { computed, onMounted, ref, shallowRef } from "vue";
import { Breadcrumb } from "@/components/forms";
import { useFountainDonationStore } from "@/stores";
import { storeToRefs } from "pinia";
import CountsCard from "./components/ProjectCountCard.vue";
import DoughnutCard from "./components/DoughnutCard.vue";

const { t } = useI18n();

const fountainDonationStore = useFountainDonationStore();
const { counts: projects } = storeToRefs(fountainDonationStore);

onMounted(() => {
  fountainDonationStore.getCounts();
});

const options = ref({
  responsive: true,
  maintainAspectRatio: false
});

const colorMap = {
  primary: "#1976D2",
  warning: "#FB8C00",
  info: "#2196F3",
  error: "#E53935"
};

const chartData = computed(() => {
  if (!projects.value?.length) return null;

  return {
    labels: projects.value.map((p) => p.project.name),
    datasets: [
      {
        data: projects.value.map((p) => p.count),
        backgroundColor: projects.value.map((p) => colorMap[p.project.color] || "#CCCCCC")
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
    <v-col cols="8" xs="12">
      <counts-card v-for="(item, index) in projects" :key="index" :item="item" class="mb-2" />
    </v-col>
    <v-col cols="4" xs="12">
      <doughnut-card :chart-data="chartData" :options="options" />
    </v-col>
  </v-row>
</template>
