<script setup async>
import { ref, onMounted } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useFountainDonationStore, usePredefinedValuesStore } from "@/stores";
import { ParentCard } from "@/components/shared/";
import { ConfirmDialog } from "@/components/forms/";
import { DetailsDialog, ActionButtons } from "./";
import config from "@/config";

const { t } = useI18n();
const fountainDonationStore = useFountainDonationStore();
const preDefinedValuesStore = usePredefinedValuesStore();
const { mediaStatuses } = storeToRefs(preDefinedValuesStore);

onMounted(async () => {
  await preDefinedValuesStore.getMediaStatuses();
});

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
const showAllInformationDialog = ref(false);
const copySuccess = ref(false);

const headers = ref([
  { title: t("admin.donations.fountains.fields.info"), key: "info", sortable: true },
  { title: t("admin.donations.fountains.fields.contact"), key: "contact", sortable: true },
  { title: t("admin.donations.fountains.fields.phone"), key: "phone", sortable: false, width: "100px" },
  {
    title: t("admin.donations.fountains.fields.creationDate"),
    key: "creationDate.displayDate",
    align: "center",
    sortable: true,
    width: "100px"
  },
  { title: t("admin.donations.fountains.fields.weeks"), key: "weeks", sortable: false, align: "center", width: "48px" },
  { title: t("admin.donations.fountains.fields.team"), key: "team", sortable: false, align: "center", width: "64px" },
  { title: t("admin.donations.fountains.fields.media"), key: "media", sortable: false, align: "center", width: "64px" },
  { title: t("admin.donations.fountains.fields.detail"), key: "actions", align: "center", sortable: false, width: "64px" }
]);

const showAllInformation = async (item) => {
  await fountainDonationStore.getById(item);
  donation.value = fountainDonationStore.donation;
  showAllInformationDialog.value = true;
};

const copyToClipboard = (text) => {
  navigator.clipboard.writeText(text).then(() => {
    copySuccess.value = true;
    setTimeout(() => {
      copySuccess.value = false;
    }, 1000);
  });
};

const changeMediaStatus = async (id, mediastatus) => {
  await fountainDonationStore.changeMediaStatus(id, mediastatus);
};

const deleteTitle = ref(null);
const deleteMessage = ref(null);
const showDeleteConfirmDialog = ref(false);
const donationId = ref(null);

const showConfirmDialog = (id) => {
  deleteTitle.value = t("admin.donations.delete.title");
  deleteMessage.value = t("admin.donations.delete.message");

  donationId.value = id;
  showDeleteConfirmDialog.value = true;
};

const abortDelete = () => {
  donationId.value = null;
  showDeleteConfirmDialog.value = false;
};

const deleteDonation = async () => {
  await fountainDonationStore.delete(donationId.value);
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
      item-value="id"
      @update:options="$emit('update:options', $event)"
    >
      <template v-slot:item="{ item }">
        <tr :key="item.id" class="donation-row">
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
            <v-chip :class="`bg-${item.status.backgroundColor}`" size="small">
              {{ item.weeks }}
            </v-chip>
          </td>
          <td>
            {{ item.team }}
          </td>
          <td>
            <div class="d-flex ga-2">
              <div
                :class="`bg-${item.mediaStatus.backgroundColor}`"
                :style="{
                  width: '12px',
                  height: '12px',
                  borderRadius: '50%'
                }"
              />
              <span>
                {{ t(`admin.donations.media.status.${item.mediaStatus.name}`) }}
              </span>
            </div>
          </td>
          <td>
            <div class="d-flex ga-2 justify-end">
              <action-buttons
                :donation-id="item.id"
                :media-statuses="mediaStatuses"
                @show-all-information="showAllInformation"
                @show-confirm-dialog="showConfirmDialog"
                @change-media-status="changeMediaStatus"
              />
            </div>
          </td>
        </tr>
      </template>
    </v-data-table-server>

    <details-dialog v-model="showAllInformationDialog" :donation="donation" />

    <confirm-dialog
      v-model="showDeleteConfirmDialog"
      :title="deleteTitle"
      :message="deleteMessage"
      @confirm="deleteDonation"
      @cancel="abortDelete"
    />
  </parent-card>
</template>

<style lang="scss" scoped>
:deep(.donations-table) {
  thead {
    th {
      background-color: rgb(var(--v-theme-surface-light));

      &:first-child {
        border-radius: 1rem 0 0 1rem;
      }

      &:last-child {
        border-radius: 0 1rem 1rem 0;
      }
      span {
        font-size: 0.75rem;
        font-weight: 700;
        text-align: center;
      }
    }
  }
  tbody {
    &:hover tr {
      opacity: 0.5;
      filter: blur(1px);
    }

    tr {
      transition: all 0.1s ease;

      &:first-child {
        td {
          padding-top: 16px;
        }
      }

      &:hover {
        position: relative;
        z-index: 10;
        box-shadow:
          0 4px 20px rgba(0, 0, 0, 0.2),
          inset 10px 0 0 0 rgb(var(--v-theme-primary));
        opacity: 1 !important;
        filter: none !important;
        cursor: pointer;
      }

      td {
        .info-wrapper-container {
          position: relative;
          display: flex;
          align-items: center;

          &:hover {
            .copy-button {
              opacity: 1;
              z-index: 2;
            }
          }

          .info-wrapper {
            overflow: auto;
            text-overflow: ellipsis;
            white-space: break-spaces;
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
