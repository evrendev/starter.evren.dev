<script setup>
import { ref, shallowRef, onMounted } from "vue";
import { useAuditStore } from "@/stores";
import { useI18n } from "vue-i18n";
const { t } = useI18n();

import BaseBreadcrumb from "@/components/shared/BaseBreadcrumb.vue";
import UiParentCard from "@/components/shared/UiParentCard.vue";
import config from "@/config";

const breadcrumbs = shallowRef([
  {
    title: t("admin.audits.title"),
    disabled: true,
    href: "#"
  }
]);

const headers = ref([
  { text: t("admin.audits.fields.id"), value: "id" },
  { text: t("admin.audits.fields.fullName"), value: "fullName" },
  { text: t("admin.audits.fields.dateTime"), value: "dateTime" },
  { text: t("admin.audits.fields.action"), value: "action" },
  { text: t("admin.audits.fields.entityType"), value: "entityType" }
]);

const items = ref([]);
const search = ref("");
const loading = ref(false);
const auditStore = useAuditStore();

onMounted(async () => {
  loading.value = true;
  await auditStore.load();
  items.value = auditStore.items;
});

const loadItems = (options) => {
  console.log(options);
};
</script>

<template>
  <BaseBreadcrumb :title="t('admin.audits.title')" :breadcrumbs="breadcrumbs"></BaseBreadcrumb>
  <v-row>
    <v-col cols="12" md="12">
      <UiParentCard :title="t('admin.audits.list')">
        <template>
          <v-data-table-server
            v-model:items-per-page="config.itemsPerPage"
            :headers="headers"
            :items="items"
            items-length="25"
            :loading="loading"
            :search="search"
            item-value="name"
            @update:options="loadItems"
          ></v-data-table-server>
        </template>
      </UiParentCard>
    </v-col>
  </v-row>
</template>
