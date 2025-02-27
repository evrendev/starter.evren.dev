<script setup>
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { useTenantStore, useAppStore } from "@/stores";
import { Breadcrumb } from "@/components/forms";
import { TenantForm } from "./components";

const { t } = useI18n();
const route = useRoute();
const tenantStore = useTenantStore();
const appStore = useAppStore();
const tenant = ref(null);

const breadcrumbs = ref([
  {
    title: t("admin.tenants.title"),
    disabled: false,
    href: "/admin/tenants/list"
  },
  {
    title: t("admin.tenants.edit"),
    disabled: true,
    href: "#"
  }
]);

onMounted(async () => {
  const id = route.params.id;

  try {
    appStore.setPageLoading(true);
    await tenantStore.getById(id);
    tenant.value = tenantStore.tenant;
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoading(false);
  }
});
</script>

<template>
  <breadcrumb :title="t('admin.tenants.edit')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <tenant-form :initial-data="tenant" :is-edit="true" />
    </v-col>
  </v-row>
</template>
