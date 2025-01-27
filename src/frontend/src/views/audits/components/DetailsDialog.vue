<script setup>
import { useI18n } from "vue-i18n";
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";

const { t } = useI18n();

defineProps({
  modelValue: {
    type: Boolean,
    required: true
  },
  audit: {
    type: Object,
    default: null
  }
});
</script>

<template>
  <v-dialog :model-value="modelValue" @update:model-value="$emit('update:modelValue', $event)" max-width="800" class="dialog-colored-bg">
    <v-card>
      <v-card-title class="text-h5">
        {{ t("admin.audits.details.title") }}
      </v-card-title>
      <v-card-text>
        <vue-json-pretty :data="audit" :show-line-number="true" />
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
