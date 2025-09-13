<script lang="ts" setup>
import { LogFilters } from "@/requests/personal";
import { Logs, Tabs } from "@/views/admin/personal";
import { usePersonalStore } from "@/stores/personal";

const personalStore = usePersonalStore();
const { loading, total, itemsPerPage, items } = storeToRefs(personalStore);

const { t } = useI18n();

const headers = computed(() => [
  ...([
    {
      title: t("admin.personal.logs.fields.type.title"),
      key: "type",
      align: "center",
      sortable: false,
    },
    {
      title: t("admin.personal.logs.fields.tableName.title"),
      key: "tableName",
      align: "center",
      sortable: true,
    },
    {
      title: t("admin.personal.logs.fields.dateTime.title"),
      key: "dateTime",
      align: "center",
      sortable: true,
    },
    {
      title: t("admin.personal.logs.fields.action.title"),
      key: "action",
      align: "center",
      sortable: false,
    },
  ] as const),
]);

const getItems = async (options: LogFilters) => {
  personalStore.setFilters(options);
  await personalStore.getLogs();
};
</script>

<template>
  <div>
    <tabs />

    <v-window class="mt-5 disable-tab-transition">
      <v-window-item>
        <logs
          :items-per-page="itemsPerPage"
          :items="items"
          :total="total"
          :loading="loading"
          :headers="headers"
          @update:options="getItems"
          item-value="id"
        />
      </v-window-item>
    </v-window>
  </div>
</template>
