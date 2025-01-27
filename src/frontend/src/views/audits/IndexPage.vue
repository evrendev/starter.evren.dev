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

// Add new refs for filters
const dateRange = ref([null, null]);
const selectedAction = ref(null);
const actions = ref([
  { title: t("common.all"), value: null },
  { title: t("admin.audits.actions.insert"), value: "Insert" },
  { title: t("admin.audits.actions.update"), value: "Update" },
  { title: t("admin.audits.actions.delete"), value: "Delete" },
  { title: t("admin.audits.actions.recovered"), value: "Recovered" }
]);

const searchOptions = {
  page: 1,
  itemsPerPage: config.itemsPerPage,
  sortBy: null,
  groupBy: null,
  action: null,
  startDate: null,
  endDate: null,
  search: null
};

// Add methods for submit and reset
const handleSubmit = () => {
  searchOptions.page = 1;
  searchOptions.action = selectedAction.value;
  searchOptions.startDate = dateRange.value[0];
  searchOptions.endDate = dateRange.value[1];
  searchOptions.search = search.value;
  loadItems(searchOptions);
};

const handleReset = () => {
  search.value = "";
  selectedAction.value = null;
  dateRange.value = [null, null];
  searchOptions.page = 1;
  searchOptions.action = null;
  searchOptions.startDate = null;
  searchOptions.endDate = null;
  searchOptions.search = null;
  loadItems(searchOptions);
};

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
      <UiParentCard class="mb-4" :title="t('common.filters')">
        <v-row>
          <v-col cols="12" md="3">
            <v-text-field v-model="search" :label="t('common.search')" density="comfortable" hide-details variant="outlined"></v-text-field>
          </v-col>
          <v-col cols="12" md="3">
            <v-select
              v-model="selectedAction"
              :items="actions"
              :label="t('admin.audits.fields.action')"
              density="comfortable"
              hide-details
              item-title="title"
              item-value="value"
              variant="outlined"
            ></v-select>
          </v-col>
          <v-col cols="12" md="6">
            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="dateRange[0]"
                  :label="t('common.selectDate')"
                  density="comfortable"
                  hide-details
                  type="date"
                  variant="outlined"
                ></v-text-field>
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="dateRange[1]"
                  :label="t('common.selectDate')"
                  density="comfortable"
                  hide-details
                  type="date"
                  variant="outlined"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
        <v-row class="mt-2">
          <v-col cols="12" class="d-flex justify-end ga-2">
            <v-btn color="error" :disabled="loading" @click="handleReset" prepend-icon="$refresh">
              {{ t("common.reset") }}
            </v-btn>
            <v-btn color="primary" :loading="loading" @click="handleSubmit" prepend-icon="$filterCheck">
              {{ t("common.submit") }}
            </v-btn>
          </v-col>
        </v-row>
      </UiParentCard>

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
