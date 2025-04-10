<script setup async>
import { ref, onMounted } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useFountainDonationStore, usePredefinedValuesStore } from "@/stores";
import { ParentCard } from "@/components/shared/";
import { ConfirmModal } from "@/components/forms/";
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
const showDonationModal = ref(false);
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

const showDonation = async (item) => {
  await fountainDonationStore.getById(item);
  donation.value = fountainDonationStore.donation;
  showDonationModal.value = true;
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

const confirmModalTitle = ref(null);
const confirmModalMessage = ref(null);
const showModal = ref(false);
const donationId = ref(null);

const showConfirmModal = (id) => {
  confirmModalTitle.value = t("admin.donations.delete.title");
  confirmModalMessage.value = t("admin.donations.delete.message");

  donationId.value = id;
  showModal.value = true;
};

const handleCancel = () => {
  donationId.value = null;
  showModal.value = false;
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
                @showDonation="showDonation"
                @showConfirmModal="showConfirmModal"
                @changeMediaStatus="changeMediaStatus"
              />
            </div>
          </td>
        </tr>
      </template>
    </v-data-table-server>

    <details-dialog v-model="showDonationModal" :donation="donation" />

    <confirm-modal
      v-model="showModal"
      :title="confirmModalTitle"
      :message="confirmModalMessage"
      @confirm="deleteDonation"
      @cancel="handleCancel"
    />
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
