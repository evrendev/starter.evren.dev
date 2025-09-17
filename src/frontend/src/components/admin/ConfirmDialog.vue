<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    showDialog: boolean;
    title: string | null;
    message: string | null;
    confirmButtonText?: string;
    cancelButtonText?: string;
    confirmButtonColor?: string;
    cancelButtonColor?: string;
  }>(),
  {
    showDialog: false,
    title: null,
    message: null,
    confirmButtonColor: "primary",
    cancelButtonColor: "secondary",
  },
);

const emit = defineEmits(["update:showDialog", "confirm", "cancel"]);

const show = computed({
  get: () => props.showDialog,
  set: (value) => emit("update:showDialog", value),
});

const onConfirm = () => {
  emit("confirm");
  emit("update:showDialog", false);
};

const onCancel = () => {
  emit("cancel");
  emit("update:showDialog", false);
};
</script>

<template>
  <v-dialog v-model="show" max-width="500px" persistent>
    <v-card>
      <v-card-title class="text-h5 bg-primary">
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
        <v-btn :color="confirmButtonColor" variant="flat" @click="onConfirm">
          {{ confirmButtonText }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
