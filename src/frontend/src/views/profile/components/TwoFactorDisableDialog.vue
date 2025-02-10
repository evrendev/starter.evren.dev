<script setup>
import { useI18n } from "vue-i18n";
import { computed } from "vue";

const { t } = useI18n();

const props = defineProps({
  modelValue: {
    type: Boolean,
    required: true
  }
});

const emit = defineEmits(["update:modelValue", "confirm"]);

const dialog = computed({
  get: () => props.modelValue,
  set: (value) => emit("update:modelValue", value)
});

const closeDialog = () => {
  emit("update:modelValue", false);
};

const handleConfirm = () => {
  emit("confirm");
};
</script>

<template>
  <v-dialog v-model="dialog" max-width="500" class="dialog-colored-title">
    <v-card>
      <v-card-title class="text-h5">
        {{ t("admin.profile.twoFactorAuth.confirmDisable.title") }}
      </v-card-title>
      <v-card-text>
        {{ t("admin.profile.twoFactorAuth.confirmDisable.message") }}
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="grey-darken-1" variant="text" @click="closeDialog">
          {{ t("common.cancel") }}
        </v-btn>
        <v-btn color="error" variant="text" @click="handleConfirm">
          {{ t("common.confirm") }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
