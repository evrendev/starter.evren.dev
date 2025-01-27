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
  { title: t("admin.audits.fields.id"), key: "id", sortable: false },
  { title: t("admin.audits.fields.user"), key: "email", sortable: false },
  { title: t("admin.audits.fields.dateTime"), key: "dateTime.displayDateWithTime", align: "center" },
  { title: t("admin.audits.fields.action"), key: "action", align: "center", sortable: false },
  { title: t("admin.audits.fields.entityType"), key: "entityType", sortable: false }
]);

const items = ref([]);
// const totalItems = ref(0);
const search = ref("");
const loading = ref(false);
const auditStore = useAuditStore();

onMounted(async () => {
  await loadItems();
});

const loadItems = async (page, itemsPerPage, sortBy, search) => {
  console.log("loadItems", page, itemsPerPage, sortBy, search);
  loading.value = true;
  await auditStore.load();
  items.value = auditStore.items;
  loading.value = false;
};
</script>

<template>
  <BaseBreadcrumb :title="t('admin.audits.title')" :breadcrumbs="breadcrumbs"></BaseBreadcrumb>
  <v-row>
    <v-col cols="12" md="12">
      <UiParentCard :title="t('admin.audits.list')">
        <v-data-table-server
          v-model:items-per-page="config.itemsPerPage"
          :headers="headers"
          :items="items"
          items-length="25"
          :loading="loading"
          item-value="id"
          :search="search"
          :load-items="loadItems"
        >
          <template v-slot:headers="{ columns }">
            <tr>
              <template v-for="column in columns" :key="column.key">
                <th class="text-center">
                  <span class="text-h4 text-no-wrap">
                    {{ column.title }}
                  </span>
                </th>
              </template>
            </tr>
          </template>
          <template #[`item.action`]="{ item }">
            <v-chip :color="item.action.backgroundColor" size="small" variant="elevated">
              {{ item.action.name }}
            </v-chip>
          </template>
        </v-data-table-server>
      </UiParentCard>
    </v-col>
  </v-row>
</template>
