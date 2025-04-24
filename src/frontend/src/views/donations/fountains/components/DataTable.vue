<script setup async>
import { ref, onMounted, computed } from "vue";
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
const { mediaStatuses, fountainTeams } = storeToRefs(preDefinedValuesStore);

onMounted(async () => {
  await preDefinedValuesStore.getMediaStatuses();
  await preDefinedValuesStore.getFountainTeams();
});

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
  },
  projectCode: {
    type: String,
    default: null
  }
});

defineEmits(["update:options"]);

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

const teamName = ref("none");
const donation = ref(null);
const showAllInformationDialog = ref(false);
const copySuccess = ref(false);
const deleteTitle = ref(null);
const deleteMessage = ref(null);
const showDeleteConfirmDialog = ref(false);
const donationId = ref(null);
const showTeamOptions = computed(() => props.projectCode == "aki" || props.projectCode == "agi");

const showAllInformation = async (id) => {
  await fountainDonationStore.getById(id);
  donation.value = fountainDonationStore.donation;
  showAllInformationDialog.value = true;
};

const openWhatsapp = (url, text) => {
  window.open(url, "_blank");
  copyToClipboard(text);
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

const changeTeam = async (id, team) => {
  await fountainDonationStore.changeTeam(id, team);
};

const showDeleteConfirmationDialog = (id) => {
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

const createEmptyDonation = async () => {
  await fountainDonationStore.createEmptyDonation({ projectCode: props.projectCode.toLocaleUpperCase(), team: teamName.value });
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
      <template v-slot:top>
        <v-toolbar :elevation="0" class="px-2 py-0 mb-2 rounded-sm border" color="surface" v-show="showTeamOptions">
          <v-toolbar-title class="text-medium-emphasis">
            <v-icon icon="$accountMultiplePlusOutline" size="x-small" class="mr-2" />
            <span class="text-h6 font-weight-bold">
              {{ t("admin.donations.fountains.fields.team") }}
            </span>
          </v-toolbar-title>

          <template #append>
            <div class="d-flex align-center ga-2">
              <v-menu>
                <template #activator="{ props }">
                  <v-btn v-bind="props" variant="outlined" size="small">
                    {{ t(`admin.donations.fountains.team.${teamName}`) }}
                    <v-icon icon="$chevronDown" size="x-small" class="ml-1" />
                  </v-btn>
                </template>

                <v-list>
                  <v-list-item v-for="team in fountainTeams" :key="team" @click.once="teamName = team.name">
                    <v-list-item-title>
                      <div class="d-flex align-center ga-1">
                        <v-avatar size="8" :color="team.backgroundColor" />
                        {{ t(`admin.donations.fountains.team.${team.name}`) }}
                      </div>
                    </v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-menu>

              <v-btn
                color="primary"
                prepend-icon="$plus"
                variant="outlined"
                class="font-medium text-caption"
                density="comfortable"
                @click.stop="createEmptyDonation"
              >
                {{ t("admin.donations.fountains.addEmptyDonation") }}
              </v-btn>
            </div>
          </template>
        </v-toolbar>
      </template>

      <template v-slot:item="{ item }">
        <tr :key="item.id" class="donation-row">
          <td>
            <div class="info-wrapper" v-html="item.htmlBanner" @click.stop="copyToClipboard(item.plainBanner)" />
            <v-tooltip activator="parent" location="top" :text="t('common.copy')" />
          </td>
          <td>
            {{ item.contact }}
          </td>
          <td>
            <v-btn v-if="item.phone" size="x-small" color="success" @click.stop="openWhatsapp(item.phone.whatsapp, item.plainBanner)">
              <v-icon icon="$whatsapp" size="small" class="mr-1" />
              {{ item.phone.formattedNumber }}
              <v-tooltip activator="parent" location="top" :text="t('common.openWhatsapp')" />
            </v-btn>
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
            <div v-if="showTeamOptions">
              <v-menu>
                <template #activator="{ props }">
                  <v-btn v-bind="props" variant="outlined" density="compact" class="d-flex w-100 text-capitalize" size="small">
                    <div class="d-flex align-center ga-1">
                      <v-avatar size="8" :color="item.team.backgroundColor" />
                      <span>{{ t(`admin.donations.fountains.team.${item.team.name}`) }}</span>
                    </div>

                    <v-icon icon="$chevronDown" size="x-small" class="ml-auto" />

                    <v-tooltip activator="parent" location="top" :text="t('common.changeTeam')" />
                  </v-btn>
                </template>

                <v-list>
                  <v-list-item v-for="team in fountainTeams" :key="team" @click.once="changeTeam(item.id, team.name)">
                    <v-list-item-title>
                      <div class="d-flex align-center ga-1">
                        <v-avatar size="8" :color="team.backgroundColor" />
                        {{ t(`admin.donations.fountains.team.${team.name}`) }}
                      </div>
                    </v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-menu>
            </div>
            <div v-else class="text-center">-</div>
          </td>
          <td>
            <v-menu>
              <template #activator="{ props }">
                <v-btn v-bind="props" variant="outlined" density="compact" class="text-capitalize w-100" size="small">
                  <v-avatar size="8" class="mr-1" :color="item.mediaStatus.backgroundColor" />
                  {{ t(`admin.donations.media.status.${item.mediaStatus.name}`) }}
                  <v-icon icon="$chevronDown" size="x-small" class="ml-1" />
                  <v-tooltip activator="parent" location="top" :text="t('common.changeMediaStatus')" />
                </v-btn>
              </template>

              <v-list>
                <v-list-item v-for="status in mediaStatuses" :key="status.name" @click.once="changeMediaStatus(item.id, status.name)">
                  <v-list-item-title>
                    <div class="d-flex align-center ga-1">
                      <v-avatar size="8" :color="status.backgroundColor" />
                      {{ t(`admin.donations.media.status.${status.name}`) }}
                    </div>
                  </v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
          </td>
          <td>
            <div class="d-flex ga-2 justify-end">
              <action-buttons
                :donation-id="item.id"
                :media-statuses="mediaStatuses"
                @show-all-information="showAllInformation"
                @show-confirm-dialog="showDeleteConfirmationDialog"
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

    <v-snackbar v-model="copySuccess" :timeout="2000" class="elevation-24" :text="copySuccess ? t('common.copied') : t('common.copy')" />
  </parent-card>
</template>

<style lang="scss" scoped>
:deep(.donations-table) {
  thead {
    th {
      background-color: rgb(var(--v-theme-lightprimary));

      &:first-child {
        border-radius: 0.25rem 0 0 0.25rem;
      }

      &:last-child {
        border-radius: 0 0.25rem 0.25rem 0;
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
      opacity: 0.75;
      filter: blur(0.5px);
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
        .info-wrapper {
          overflow: auto;
          text-overflow: ellipsis;
          white-space: break-spaces;
        }
      }
    }
  }
}
</style>
