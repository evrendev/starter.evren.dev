<script setup>
import { shallowRef, ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { useFountainDonationStore, useAppStore } from "@/stores";
import { Breadcrumb } from "@/components/forms";
import { FountainForm } from "./components/";

const { t } = useI18n();

const route = useRoute();
const donationStore = useFountainDonationStore();
const appStore = useAppStore();
const donation = ref(null);

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
    title: t("admin.donations.edit"),
    disabled: true,
    href: "#"
  }
]);

onMounted(async () => {
  const id = route.params.id;

  try {
    appStore.setLoading(true);
    await donationStore.getById(id);
    donation.value = donationStore.donation;
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setLoading(false);
  }
});
</script>

<template>
  <breadcrumb :title="t('admin.donations.edit')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <fountain-form v-if="donation" :initial-data="donation" :is-edit="true" />
    </v-col>
  </v-row>
</template>
