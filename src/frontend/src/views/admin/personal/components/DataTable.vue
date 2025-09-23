<script setup lang="ts">
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

const emit = defineEmits<{
  (e: "update:options", options: any): void;
}>();
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
      </v-data-table-server>
    </v-card-text>
  </v-card>
</template>
