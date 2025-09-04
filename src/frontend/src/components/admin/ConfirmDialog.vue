<script setup lang="ts">
const { t } = useI18n();
const props = withDefaults(
  defineProps<{
    modelValue: boolean;
    title: string | null;
    message: string | null;
    confirmButtonText?: string;
    cancelButtonText?: string;
    confirmButtonColor?: string;
    cancelButtonColor?: string;
  }>(),
  {
    modelValue: false,
    title: null,
    message: null,
    confirmButtonColor: "primary",
    cancelButtonColor: "secondary",
  },
);

const emit = defineEmits(["update:modelValue", "confirm", "cancel"]);

const dialog = computed({
  get: () => props.modelValue,
  set: (value) => emit("update:modelValue", value),
});

const onConfirm = () => {
  emit("confirm");
  emit("update:modelValue", false);
};

const onCancel = () => {
  emit("cancel");
  emit("update:modelValue", false);
};
</script>

<template>
  <v-dialog
    v-model="dialog"
    max-width="500px"
    persistent
    class="dialog-colored-title"
  >
    <v-card>
      <v-card-title class="text-h5">
        {{ title }}
      </v-card-title>

      <v-card-text>
        {{ message }}
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn :color="cancelButtonColor" variant="text" @click="onCancel">
          {{ cancelButtonText }}
        </v-btn>
        <v-btn
          :color="confirmButtonColor"
          variant="elevated"
          @click="onConfirm"
        >
          {{ confirmButtonText }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
