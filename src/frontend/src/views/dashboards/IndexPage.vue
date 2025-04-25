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
  primary: "#009846",
  warning: "#ffc107",
  info: "#03c9d7",
  error: "#f44336"
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
    <v-col lg="8" sm="12" md="9">
      <v-sheet class="pa-2 ma-2">
        <counts-card v-for="(item, index) in projects" :key="index" :item="item" class="mb-2" />
      </v-sheet>
    </v-col>
    <v-col lg="4" sm="12" md="3">
      <v-sheet class="pa-2 ma-2">
        <doughnut-card :chart-data="chartData" :options="options" />
      </v-sheet>
    </v-col>
  </v-row>
</template>
