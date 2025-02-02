<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import BaseBreadcrumb from "@/components/shared/BaseBreadcrumb.vue";
import TenantForm from "./components/TenantForm.vue";

const { t } = useI18n();

const breadcrumbs = ref([
  {
    title: t("admin.tenants.title"),
    disabled: false,
    href: "/admin/tenants"
  },
  {
    title: t("admin.tenants.new"),
    disabled: true,
    href: "#"
  }
]);
</script>

<template>
  <base-breadcrumb :title="t('admin.tenants.new')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <tenant-form :is-edit="false" />
    </v-col>
  </v-row>
</template>
