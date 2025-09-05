<script setup lang="ts">
const props = defineProps<{
  showModal: boolean;
  title: string | null;
}>();

const emit = defineEmits(["update:toggleModal"]);

const show = computed({
  get: () => props.showModal,
  set: (value) => emit("update:toggleModal", value),
});
</script>

<template>
  <v-dialog v-model="show" max-width="500px" persistent>
    <v-card>
      <v-card-title class="text-h5 bg-primary">
        {{ title }}
      </v-card-title>

      <v-card-text>
        <slot name="content"> </slot>
      </v-card-text>

      <v-card-actions>
        <slot name="action-buttons"></slot>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
