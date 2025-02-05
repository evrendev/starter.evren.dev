<script setup>
import { ref, watch, shallowRef } from "vue";
import { useRoleStore } from "@/stores";
import { useI18n } from "vue-i18n";
import { storeToRefs } from "pinia";
import { DataTable } from "./components";
import { Breadcrumb } from "@/components/forms";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.roles.title"),
    disabled: true,
    href: "#"
  }
]);

const items = ref([]);
const itemsLength = ref(0);
const loading = ref(false);
const roleStore = useRoleStore();
const { reset } = storeToRefs(roleStore);

watch(
  () => reset.value,
  () => {
    if (reset.value) {
      loading.value = true;
      reloadTable(1000);
    }
  }
);

const searchOptions = ref({});

const reloadTable = (timeout) => {
  loading.value = true;

  setTimeout(() => {
    getItems(searchOptions.value);
  }, timeout);
};

const getItems = async (options) => {
  loading.value = true;
  searchOptions.value = options;

  await roleStore.getItems(options);
  items.value = roleStore.items;
  itemsLength.value = roleStore.itemsLength;
  loading.value = false;
};
</script>

<template>
  <breadcrumb :title="t('admin.roles.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <data-table :items="items" :items-length="itemsLength" :loading="loading" @update:options="getItems" />
    </v-col>
  </v-row>
</template>
