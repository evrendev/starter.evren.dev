<script setup>
import { useI18n } from "vue-i18n";
const { t } = useI18n();

defineProps({
  item: {
    type: Object,
    default: null,
    required: true
  }
});
</script>

<template>
  <v-card elevation="0" :class="`bg-${item?.project.color} overflow-hidden`">
    <v-card-text>
      <h2 class="text-h1 font-weight-medium" v-text="`${item?.count} ${t('common.count')}`" />
      <span class="text-subtitle-1 text-medium-emphasis text-white" v-text="item?.project.name" />
    </v-card-text>
  </v-card>
</template>
