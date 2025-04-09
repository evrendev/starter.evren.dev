<script setup>
import { shallowRef } from "vue";
import { useI18n } from "vue-i18n";
import { Breadcrumb } from "@/components/forms";
import { FountainForm } from "./components/";

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
    title: t("admin.donations.new"),
    disabled: true,
    href: "#"
  }
]);
</script>

<template>
  <breadcrumb :title="t('admin.donations.new')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <fountain-form />
    </v-col>
  </v-row>
</template>
