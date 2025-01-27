<script setup>
import { ref, shallowRef } from "vue";
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
  { title: t("admin.audits.fields.id"), key: "id", sortable: true },
  { title: t("admin.audits.fields.user"), key: "email", sortable: false },
  { title: t("admin.audits.fields.dateTime"), key: "dateTime.displayDateWithTime", align: "center", sortable: false },
  { title: t("admin.audits.fields.action"), key: "action", align: "center", sortable: false },
  { title: t("admin.audits.fields.entityType"), key: "entityType", sortable: false }
]);

const items = ref([]);
const itemsLength = ref(0);
const search = ref("");
const loading = ref(false);
const auditStore = useAuditStore();

const loadItems = async (options) => {
  loading.value = true;
  await auditStore.load(options);
  items.value = auditStore.items;
  itemsLength.value = auditStore.itemsLength;
  loading.value = false;
};
</script>

<template>
  <BaseBreadcrumb :title="t('admin.audits.title')" :breadcrumbs="breadcrumbs"></BaseBreadcrumb>
  <v-row>
    <v-col cols="12" md="12">
      <UiParentCard>
        <v-data-table-server
          :items-per-page="config.itemsPerPage"
          :headers="headers"
          :items="items"
          :items-length="itemsLength"
          :loading="loading"
          item-value="id"
          :search="search"
          @update:options="loadItems"
        >
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

<style type="scss">
.v-data-table {
  &.v-theme--LightTheme {
    tbody {
      tr {
        &:nth-of-type(even) {
          background-color: rgba(0, 0, 0, 0.05);
        }
      }
    }
  }

  &.v-theme--DarkTheme {
    tbody {
      tr {
        &:nth-of-type(even) {
          background-color: rgba(255, 255, 255, 0.05);
        }
      }
    }
  }
}
</style>
