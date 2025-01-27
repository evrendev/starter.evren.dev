<script setup>
import { shallowRef } from "vue";
import { useI18n } from "vue-i18n";
import BaseBreadcrumb from "@/components/shared/BaseBreadcrumb.vue";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.tenants.title"),
    disabled: true,
    href: "#"
  }
]);
</script>

<template>
  <base-breadcrumb :title="t('admin.tenants.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12"> New Tenants </v-col>
  </v-row>
</template>
