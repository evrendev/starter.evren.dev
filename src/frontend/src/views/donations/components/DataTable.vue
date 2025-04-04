<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useDonationStore } from "@/stores";
import { ParentCard } from "@/components/shared/";
import { DetailsDialog } from "./";
import config from "@/config";

const { t } = useI18n();
const donationStore = useDonationStore();

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

const donation = ref(null);
const showDetailsModal = ref(false);
const detailsLoading = ref(false);

const headers = ref([
  { title: t("admin.donations.fields.contact"), key: "contact", sortable: true, width: "150px" },
  { title: t("admin.donations.fields.phone"), key: "phone", sortable: false, width: "100px" },
  { title: t("admin.donations.fields.creationDate"), key: "creationDate.displayDate", align: "center", sortable: true, width: "100px" },
  { title: t("admin.donations.fields.info"), key: "info", sortable: true },
  { title: t("admin.donations.fields.detail"), key: "actions", align: "center", sortable: false, width: "64px" }
]);

const showDetails = async (item) => {
  detailsLoading.value = true;
  await donationStore.getById(item);
  donation.value = donationStore.donation;
  showDetailsModal.value = true;
};
</script>

<template>
  <parent-card>
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
      <template #[`item.info`]="{ item }">
        <div class="info-wrapper">
          {{ item.info }}
        </div>
      </template>
      <template #[`item.actions`]="{ item }">
        <v-icon @click="showDetails(item.id)" :loading="detailsLoading" size="small" icon="$magnifyExpand" />
      </template>
    </v-data-table-server>

    <details-dialog v-model="showDetailsModal" :donation="donation" />
  </parent-card>
</template>

<style lang="scss" scoped>
.info-wrapper {
  max-width: 250px;
  overflow: auto;
  text-overflow: ellipsis;
  white-space: break-spaces;
}
</style>
