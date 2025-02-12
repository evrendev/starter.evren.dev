<script setup>
import { ref, shallowRef } from "vue";
import { storeToRefs } from "pinia";
import { useUserStore, useAuthStore } from "@/stores";
import { useI18n } from "vue-i18n";
import { FilterCard, DataTable } from "./components/";
import { Breadcrumb } from "@/components/forms";
import config from "@/config";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.users.title"),
    disabled: true,
    href: "#"
  }
]);

const items = ref([]);
const itemsLength = ref(0);
const loading = ref(false);
const userStore = useUserStore();
const authStore = useAuthStore();

const { user } = storeToRefs(authStore);
const adminPermissions = user.value?.permissions || [];
const hasUserDeletePermission = adminPermissions.includes("Users.Delete");
const hasUserRestorePermission = adminPermissions.includes("Users.Restore");

const searchOptions = {
  page: 1,
  itemsPerPage: config.itemsPerPage,
  sortBy: null,
  groupBy: null,
  startDate: null,
  endDate: null,
  search: null,
  showDeletedItems: false
};

const handleFilterSubmit = (filters) => {
  searchOptions.page = 1;
  searchOptions.startDate = filters.startDate;
  searchOptions.endDate = filters.endDate;
  searchOptions.search = filters.search;
  searchOptions.showDeletedItems = filters.showDeletedItems;

  getItems(searchOptions);
};

const handleFilterReset = () => {
  searchOptions.page = 1;
  searchOptions.startDate = null;
  searchOptions.endDate = null;
  searchOptions.search = null;
  searchOptions.showDeletedItems = false;

  getItems(searchOptions);
};

const getItems = async (options) => {
  loading.value = true;
  searchOptions.value = options;
  await userStore.getItems(options);
  items.value = userStore.items;
  itemsLength.value = userStore.itemsLength;
  loading.value = false;
};
</script>

<template>
  <breadcrumb :title="t('admin.users.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <filter-card
        :loading="loading"
        :has-user-delete-permission="hasUserDeletePermission"
        :has-user-restore-permission="hasUserRestorePermission"
        @submit="handleFilterSubmit"
        @reset="handleFilterReset"
      />

      <data-table
        :items="items"
        :items-length="itemsLength"
        :loading="loading"
        :has-user-delete-permission="hasUserDeletePermission"
        :has-user-restore-permission="hasUserRestorePermission"
        @update:options="getItems"
      />
    </v-col>
  </v-row>
</template>
