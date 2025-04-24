<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps({
  modelValue: {
    type: Boolean,
    required: true
  },
  mediaStatus: {
    type: String,
    default: null
  }
});

const mediaInformation = ref(null);

const emit = defineEmits(["update:modelValue", "save:mediaInformation"]);

const saveMediaInformation = () => {
  emit("save:mediaInformation", mediaInformation.value);
  emit("update:modelValue", false);

  mediaInformation.value = null;
};
</script>

<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="800"
    class="dialog-colored-title"
    v-if="modelValue"
  >
    <v-card>
      <v-card-title class="text-h5"> {{ t(`admin.donations.media.status.${mediaStatus}`) }} </v-card-title>
      <v-card-text>
        <v-textarea :label="t('admin.donations.fountains.details.mediaInformation')" v-model="mediaInformation"></v-textarea>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" @click="saveMediaInformation" prepend-icon="$contentSave" variant="elevated">
          {{ t("common.save") }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
