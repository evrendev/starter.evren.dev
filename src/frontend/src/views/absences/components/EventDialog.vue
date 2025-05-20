<script setup>
import { useI18n } from "vue-i18n";
import EventForm from "./EventForm.vue";

const { t } = useI18n();

defineProps({
  event: {
    type: Object,
    required: false,
    default: () => null
  },
  showEventDialog: {
    type: Boolean,
    required: true,
    default: false
  }
});

const emits = defineEmits(["saveEvent", "deleteEvent", "closeDialog"]);

const saveEvent = (event) => {
  emits("saveEvent", event);
};

const deleteEvent = (eventId) => {
  emits("deleteEvent", eventId);
};

const closeDialog = () => {
  emits("closeDialog");
};
</script>

<template>
  <v-dialog :model-value="showEventDialog" @update:model-value="closeDialog" max-width="800" class="dialog-colored-title">
    <v-card>
      <v-card-title class="text-h4">
        {{ t("admin.absences.new") }}
      </v-card-title>
      <v-card-text>
        <event-form @save-event="saveEvent" @delete-event="deleteEvent" :event="event" />
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
