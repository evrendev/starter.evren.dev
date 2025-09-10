<script lang="ts" setup>
import { Log } from "@/models/user";
import { Props } from "@/requests/app";
const { t } = useI18n();

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
        :items-per-page="itemsPerPage"
        :items="items"
        :items-length="total"
        :item-value="itemValue"
        :headers="headers"
        :loading="loading"
        @update:options="emit('update:options', $event)"
        class="striped border"
      />
    </v-card-text>
  </v-card>
</template>

<style lang="scss" scoped>
.v-table {
  th {
    text-align: start !important;
  }
}
</style>
