<script setup lang="ts">
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";

import { Log } from "@/models/user";
import { Props } from "@/types/requests/app";
import { useDateFormat } from "@vueuse/core";
const { t, locale } = useI18n();

withDefaults(defineProps<Props<Log>>(), {
  itemsPerPage: 25,
  items: () => [],
  total: 0,
  loading: false,
  headers: () => [],
});

const log: Ref<Log | null> = ref(null);
const showLogDetailsModal = ref(false);

const emit = defineEmits<{
  (e: "update:options", options: any): void;
}>();

const showDetails = (item: Log) => {
  log.value = item;
  showLogDetailsModal.value = true;
};
</script>

<template>
  <v-card elevation="6" class="mt-4" :title="t('admin.personal.logs.title')">
    <v-card-text>
      <v-data-table-server
        class="striped border"
        :items-per-page="itemsPerPage"
        :items="items"
        :items-length="total"
        :item-value="itemValue"
        :headers="headers"
        :loading="loading"
        @update:options="emit('update:options', $event)"
      >
        <template #[`item.dateTime`]="{ item }">
          {{
            useDateFormat(item.dateTime, "DD MMM YYYY HH:mm", {
              locales: locale,
            })
          }}
        </template>

        <template #[`item.action`]="{ item }">
          <v-btn @click="showDetails(item)" prepend-icon="bx-show" text>
            {{ t("admin.personal.logs.showDetails") }}
          </v-btn>
        </template>
      </v-data-table-server>
    </v-card-text>
  </v-card>

  <modal-window
    width="80vw"
    :show-modal="showLogDetailsModal"
    :title="t('admin.personal.logs.title')"
    @update:toggle-modal="() => {}"
  >
    <template #content>
      <v-table striped="odd" density="compact">
        <tbody>
          <tr v-if="log?.id">
            <td class="font-weight-medium">
              {{ t("admin.personal.logs.fields.id.title") }}:
            </td>
            <td>{{ log?.id }}</td>
          </tr>
          <tr v-if="log?.dateTime">
            <td class="font-weight-medium">
              {{ t("admin.personal.logs.fields.dateTime.title") }}:
            </td>
            <td>
              {{
                useDateFormat(log?.dateTime, "DD MMM YYYY HH:mm", {
                  locales: locale,
                })
              }}
            </td>
          </tr>
          <tr v-if="log?.affectedColumns">
            <td class="font-weight-medium">
              {{ t("admin.personal.logs.fields.affectedColumns.title") }}:
            </td>
            <td>{{ JSON.parse(log?.affectedColumns ?? "[]").join(", ") }}</td>
          </tr>
          <tr v-if="log?.primaryKey">
            <td class="font-weight-medium">
              {{ t("admin.personal.logs.fields.primaryKey.title") }}:
            </td>
            <td>{{ JSON.parse(log?.primaryKey ?? "{}").id }}</td>
          </tr>
          <tr v-if="log?.tableName">
            <td class="font-weight-medium">
              {{ t("admin.personal.logs.fields.tableName.title") }}:
            </td>
            <td>{{ log?.tableName }}</td>
          </tr>
          <tr v-if="log?.oldValues">
            <td class="font-weight-medium">
              {{ t("admin.personal.logs.fields.oldValues.title") }}:
            </td>
            <td>
              <code>
                <vue-json-pretty
                  :data="JSON.parse(log?.oldValues ?? '{}')"
                  :show-line-number="true"
                  :show-double-quotes="true"
                />
              </code>
            </td>
          </tr>
          <tr v-if="log?.newValues">
            <td class="font-weight-medium">
              {{ t("admin.personal.logs.fields.newValues.title") }}:
            </td>
            <td>
              <code>
                <vue-json-pretty
                  :data="JSON.parse(log?.newValues ?? '')"
                  :show-line-number="true"
                  :show-double-quotes="true"
                />
              </code>
            </td>
          </tr>
        </tbody>
      </v-table>
    </template>

    <template #action-buttons>
      <v-btn
        @click="showLogDetailsModal = false"
        v-text="t('shared.close')"
        variant="flat"
        size="small"
        color="primary"
      />
    </template>
  </modal-window>
</template>
