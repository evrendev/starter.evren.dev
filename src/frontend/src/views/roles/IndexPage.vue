<script setup>
import { ref, shallowRef } from "vue";
import { useRoleStore } from "@/stores";
import { useI18n } from "vue-i18n";
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

const getItems = async (options) => {
  loading.value = true;
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
