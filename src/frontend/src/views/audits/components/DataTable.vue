<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useAuditStore } from "@/stores";
import UiParentCard from "@/components/shared/UiParentCard.vue";
import DetailsDialog from "./DetailsDialog.vue";
import config from "@/config";

const { t } = useI18n();
const auditStore = useAuditStore();

defineProps({
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

defineEmits(["update:options"]);

const audit = ref(null);
const showDetailsModal = ref(false);
const detailsLoading = ref(false);

const headers = ref([
  { title: t("admin.audits.fields.user"), key: "email", sortable: false },
  { title: t("admin.audits.fields.dateTime"), key: "dateTime.displayDateWithTime", align: "center", sortable: true },
  { title: t("admin.audits.fields.action"), key: "action", align: "center", sortable: false },
  { title: t("admin.audits.fields.entityType"), key: "entityType", sortable: false },
  { title: t("admin.audits.fields.detail"), key: "actions", align: "center", sortable: false }
]);

const showDetails = async (item) => {
  detailsLoading.value = true;
  await auditStore.getById(item);
  audit.value = auditStore.audit;
  showDetailsModal.value = true;
};
</script>

<template>
  <UiParentCard>
    <v-data-table-server
      :items-per-page="config.itemsPerPage"
      :headers="headers"
      :items="items"
      :items-length="itemsLength"
      :loading="loading"
      class="striped"
      item-value="id"
      @update:options="$emit('update:options', $event)"
    >
      <template #[`item.action`]="{ item }">
        <v-chip :color="item.action.backgroundColor" size="small" variant="elevated">
          {{ item.action.name }}
        </v-chip>
      </template>

      <template #[`item.actions`]="{ item }">
        <v-icon @click="showDetails(item.id)" :loading="detailsLoading" size="small" icon="$magnifyExpand" />
      </template>
    </v-data-table-server>

    <details-dialog v-model="showDetailsModal" :audit="audit" />
  </UiParentCard>
</template>
