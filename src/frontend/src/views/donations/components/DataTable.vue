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
const copySuccess = ref(false);

const headers = ref([
  { title: t("admin.donations.fields.contact"), key: "contact", sortable: true, width: "150px" },
  { title: t("admin.donations.fields.phone"), key: "phone", sortable: false, width: "100px" },
  { title: t("admin.donations.fields.creationDate"), key: "creationDate.displayDate", align: "center", sortable: true, width: "100px" },
  { title: t("admin.donations.fields.info"), key: "info", sortable: true },
  { title: t("admin.donations.fields.weeks"), key: "weeks", sortable: false, align: "center" },
  { title: t("admin.donations.fields.team"), key: "team", sortable: false, align: "center" },
  { title: t("admin.donations.fields.detail"), key: "actions", align: "center", sortable: false, width: "64px" }
]);

const showDetails = async (item) => {
  detailsLoading.value = true;
  await donationStore.getById(item);
  donation.value = donationStore.donation;
  showDetailsModal.value = true;
};

const copyToClipboard = (text) => {
  navigator.clipboard.writeText(text).then(() => {
    copySuccess.value = true;
    setTimeout(() => {
      copySuccess.value = false;
    }, 1000);
  });
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
        <div class="info-wrapper-container">
          <div class="info-wrapper">
            {{ item.info }}
          </div>
          <v-btn
            icon
            class="copy-button"
            size="x-small"
            :color="copySuccess ? 'primary' : 'default'"
            @click.stop="copyToClipboard(item.info)"
          >
            <v-icon :icon="copySuccess ? '$check' : '$contentCopy'" size="small" />
            <v-tooltip activator="parent" location="top">
              {{ copySuccess ? t("common.copied") : t("common.copy") }}
            </v-tooltip>
          </v-btn>
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
.info-wrapper-container {
  position: relative;
  display: flex;
  align-items: center;

  &:hover {
    .copy-button {
      opacity: 1;
    }
  }

  .info-wrapper {
    overflow: auto;
    text-overflow: ellipsis;
    white-space: break-spaces;
    padding-right: 40px;
  }

  .copy-button {
    position: absolute;
    right: 0;
    opacity: 0;
    transition: opacity 0.2s ease;
  }
}
</style>
