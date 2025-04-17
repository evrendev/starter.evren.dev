<script setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps({
  donationId: {
    type: String,
    default: null,
    required: true
  },
  mediaStatuses: {
    type: Array,
    default: () => [],
    required: true
  }
});

defineEmits(["showAllInformation", "showConfirmDialog"]);
</script>
<template>
  <div class="d-flex ga-2 justify-end">
    <v-btn icon density="compact" @click.stop="$emit('showAllInformation', donationId)">
      <v-icon size="x-small">$information</v-icon>
      <v-tooltip activator="parent" location="top" :text="t('common.showDetails')" />
    </v-btn>

    <v-btn icon density="compact" :to="{ name: 'edit-fountain', params: { id: donationId } }">
      <v-icon size="x-small">$pencil</v-icon>
      <v-tooltip activator="parent" location="top" :text="t('common.update')" />
    </v-btn>

    <v-btn icon density="compact" @click.stop="$emit('showConfirmDialog', donationId)">
      <v-icon size="x-small">$trashCan</v-icon>
      <v-tooltip activator="parent" location="top" :text="t('common.delete')" />
    </v-btn>
  </div>
</template>
