<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useFontainDonationStore } from "@/stores";
import { ParentCard } from "@/components/shared/";
import { DetailsDialog } from "./";
import config from "@/config";

const { t } = useI18n();
const fountainDonationStore = useFontainDonationStore();

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
const copySuccess = ref(false);

const headers = ref([
  { title: t("admin.donations.fontains.fields.info"), key: "info", sortable: true },
  { title: t("admin.donations.fontains.fields.contact"), key: "contact", sortable: true },
  { title: t("admin.donations.fontains.fields.phone"), key: "phone", sortable: false, width: "100px" },
  {
    title: t("admin.donations.fontains.fields.creationDate"),
    key: "creationDate.displayDate",
    align: "center",
    sortable: true,
    width: "100px"
  },
  { title: t("admin.donations.fontains.fields.weeks"), key: "weeks", sortable: false, align: "center", width: "48px" },
  { title: t("admin.donations.fontains.fields.team"), key: "team", sortable: false, align: "center", width: "64px" },
  { title: t("admin.donations.fontains.fields.detail"), key: "actions", align: "center", sortable: false, width: "64px" }
]);

const showDetails = async (item) => {
  await fountainDonationStore.getById(item);
  donation.value = fountainDonationStore.donation;
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
      class="text-caption striped donations-table"
      density="compact"
      item-value="id"
      @update:options="$emit('update:options', $event)"
    >
      <template v-slot:item="{ item }">
        <tr>
          <td>
            <div class="info-wrapper-container">
              <div class="info-wrapper" v-html="item.htmlBanner" />
              <v-btn
                icon
                class="copy-button"
                size="x-small"
                :color="copySuccess ? 'primary' : 'default'"
                @click.stop="copyToClipboard(item.plainBanner)"
              >
                <v-icon :icon="copySuccess ? '$check' : '$contentCopy'" size="small" />
                <v-tooltip activator="parent" location="top">
                  {{ copySuccess ? t("common.copied") : t("common.copy") }}
                </v-tooltip>
              </v-btn>
            </div>
          </td>
          <td>
            {{ item.contact }}
          </td>
          <td>
            {{ item.phone }}
          </td>
          <td>
            {{ item.creationDate.displayDate }}
          </td>
          <td class="text-center">
            <v-chip :class="`bg-${item.status.backgroundColor}`" size="small" density="compact">
              {{ item.weeks }}
            </v-chip>
          </td>
          <td>
            {{ item.team }}
          </td>
          <td>
            <v-btn icon size="x-small" density="compact" @click.stop="showDetails(item.id)">
              <v-icon icon="$magnifyExpand" />
              <v-tooltip activator="parent" location="top">
                {{ t("common.showDetails") }}
              </v-tooltip>
            </v-btn>
          </td>
        </tr>
      </template>
    </v-data-table-server>

    <details-dialog v-model="showDetailsModal" :donation="donation" />
  </parent-card>
</template>

<style lang="scss" scoped>
:deep(.donations-table) {
  tbody {
    tr {
      &:first-child {
        td {
          padding-top: 16px;
        }
      }

      td {
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
      }
    }
  }
}
</style>
