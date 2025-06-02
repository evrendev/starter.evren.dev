<script setup>
import { useI18n } from "vue-i18n";
import { ref, shallowRef } from "vue";
import { Breadcrumb } from "@/components/forms";

const { t } = useI18n();
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
</template>
