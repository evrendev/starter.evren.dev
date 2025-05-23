<script setup>
import { shallowRef, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { useFountainDonationStore, useAppStore } from "@/stores";
import { storeToRefs } from "pinia";
import { Breadcrumb } from "@/components/forms";
import { ReportCard } from "./components/";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.donations.title"),
    disabled: true,
    href: "/admin/donations"
  },
  {
    title: t("admin.donations.fountains.title"),
    disabled: true,
    href: "#"
  },
  {
    title: t("admin.donations.fountains.weeklyReport.title"),
    disabled: true,
    href: "#"
  }
]);

const appStore = useAppStore();
const fountainDonationStore = useFountainDonationStore();
const { loading } = storeToRefs(appStore);
const { isoYear, isoWeekNumber, projects } = storeToRefs(fountainDonationStore);

onMounted(() => {
  fountainDonationStore.getWeeklyReports();
});
</script>

<template>
  <breadcrumb :title="t('admin.donations.fountains.weeklyReport.title')" :breadcrumbs="breadcrumbs" />
  <report-card v-if="!loading" :projects="projects" :iso-year="isoYear" :iso-week-number="isoWeekNumber" />
</template>
