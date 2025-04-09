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
              <td>{{ donation.banner }}</td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.contact") }}</td>
              <td>{{ donation.contact }}</td>
            </tr>
            <tr>
              <td>{{ t("admin.donations.fountains.details.phone") }}</td>
              <td>{{ donation.phone }}</td>
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
              <td>{{ t("admin.donations.fountains.details.transactionId") }}</td>
              <td>{{ donation.transactionId }}</td>
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
