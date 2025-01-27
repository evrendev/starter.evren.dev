<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import UiParentCard from "@/components/shared/UiParentCard.vue";
import config from "@/config";

const { t } = useI18n();

const props = defineProps({
  items: {
    type: Array,
    required: true
  },
  itemsLength: {
    type: Number,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(["update:options"]);

const headers = ref([
  { title: t("admin.audits.fields.id"), key: "id", sortable: true },
  { title: t("admin.audits.fields.user"), key: "email", sortable: false },
  { title: t("admin.audits.fields.dateTime"), key: "dateTime.displayDateWithTime", align: "center", sortable: false },
  { title: t("admin.audits.fields.action"), key: "action", align: "center", sortable: false },
  { title: t("admin.audits.fields.entityType"), key: "entityType", sortable: false }
]);
</script>

<template>
  <UiParentCard>
    <v-data-table-server
      :items-per-page="config.itemsPerPage"
      :headers="headers"
      :items="items"
      :items-length="itemsLength"
      :loading="loading"
      item-value="id"
      @update:options="$emit('update:options', $event)"
    >
      <template #[`item.action`]="{ item }">
        <v-chip :color="item.action.backgroundColor" size="small" variant="elevated">
          {{ item.action.name }}
        </v-chip>
      </template>
    </v-data-table-server>
  </UiParentCard>
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
