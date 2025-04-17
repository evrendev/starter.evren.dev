<script setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps({
  modelValue: {
    type: Boolean,
    required: true
  },
  donation: {
    type: Object,
    default: null
  }
});

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
</script>

<template>
  <v-dialog :model-value="modelValue" @update:model-value="$emit('update:modelValue', $event)" max-width="800" class="dialog-colored-title">
    <v-card>
      <v-card-title class="text-h5"> {{ donation.transactionId }} - {{ donation.contact }} </v-card-title>
      <v-card-text>
        <v-table height="300px">
          <tbody>
            <tr>
              <td>{{ t("admin.donations.fountains.details.creationDate") }}</td>
              <td>{{ donation.creationDate.displayDate }}</td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.weeks") }}</td>
              <td>
                <div :class="`bg-${donation.status.backgroundColor}`" class="text-center">
                  {{ `${donation.weeks} - ${t(`admin.donations.fountains.status.${donation.status.name}`)}` }}
                </div>
              </td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.banner") }}</td>
              <td><div v-html="donation.htmlBanner" /></td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.contact") }}</td>
              <td>{{ donation.contact }}</td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.phone") }}</td>
              <td>
                <v-btn size="x-small" color="success" @click.stop="openWhatsapp(donation.phone.whatsapp, donation.plainBanner)">
                  <v-icon icon="$whatsapp" size="small" class="mr-1" />
                  {{ donation.phone.formattedNumber }}
                  <v-tooltip activator="parent" location="top" :text="t('common.openWhatsapp')" />
                </v-btn>
              </td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.project") }}</td>
              <td>{{ donation.project }}</td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.projectCode") }}</td>
              <td>{{ donation.projectCode }}</td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.projectNumber") }}</td>
              <td>{{ donation.projectNumber }}</td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.mediaStatus") }}</td>
              <td>
                <div :class="`bg-${donation.mediaStatus.backgroundColor}`" class="text-center">
                  {{ t(`admin.donations.media.status.${donation.mediaStatus.name}`) }}
                </div>
              </td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.transactionId") }}</td>
              <td>{{ donation.transactionId ?? "-" }}</td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.source") }}</td>
              <td>{{ donation.source }}</td>
            </tr>
          </tbody>
        </v-table>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" @click="$emit('update:modelValue', false)" prepend-icon="$close" variant="elevated">
          {{ t("common.close") }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
