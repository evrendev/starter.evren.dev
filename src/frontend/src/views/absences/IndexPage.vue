<script setup>
import { shallowRef, ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { Breadcrumb } from "@/components/forms";
import { useAbsenceStore } from "@/stores";
import { storeToRefs } from "pinia";
import { EventDialog, CalendarApp } from "./components/";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.absences.title"),
    disabled: true,
    href: "#"
  }
]);

const loading = ref(true);
const event = ref(null);
const showEventDialog = ref(false);
const absenceStore = useAbsenceStore();
const { events } = storeToRefs(absenceStore);

onMounted(async () => {
  await absenceStore.getEvents();
  loading.value = false;
});

const saveEvent = async (event) => {
  loading.value = true;
  await absenceStore.save(event);
  loading.value = false;
  showEventDialog.value = false;
};

const deleteEvent = async (eventId) => {
  loading.value = true;
  await absenceStore.delete(eventId);
  loading.value = false;
  showEventDialog.value = false;
};

const showEvent = (values) => {
  showEventDialog.value = true;
  event.value = values;
};

const closeDialog = () => {
  showEventDialog.value = false;
};
</script>

<template>
  <breadcrumb :title="t('admin.absences.title')" :breadcrumbs="breadcrumbs" />
  <calendar-app v-if="!loading" :loading="loading" :events="events" @show-event-dialog="showEvent" />
  <event-dialog
    :showEventDialog="showEventDialog"
    :event="event"
    @save-event="saveEvent"
    @delete-event="deleteEvent"
    @close-dialog="closeDialog"
  />
</template>
