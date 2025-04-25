<script setup>
import { useI18n } from "vue-i18n";
import { onMounted, ref, shallowRef } from "vue";
import { Breadcrumb } from "@/components/forms";
import { useFountainDonationStore } from "@/stores";
import { storeToRefs } from "pinia";
import CountsCard from "./components/ProjectCountCard.vue";

const { t } = useI18n();

const fountainDonationStore = useFountainDonationStore();
const { counts: projects } = storeToRefs(fountainDonationStore);

onMounted(() => {
  fountainDonationStore.getCounts();
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
    <v-col cols="3" xs="12" v-for="(item, index) in projects" :key="index">
      <counts-card :item="item" />
    </v-col>
  </v-row>
</template>
