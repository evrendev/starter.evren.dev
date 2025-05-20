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
const render = ref(false);
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
  render.value = true;
};

const showEvent = (values) => {
  showEventDialog.value = true;
  event.value = values;
};
</script>

<template>
  <breadcrumb :title="t('admin.absences.title')" :breadcrumbs="breadcrumbs" />
  <calendar-app :loading="loading" :render="render" @show-event-dialog="showEvent" :events="events" />
  <event-dialog :showEventDialog="showEventDialog" :event="event" @save="saveEvent" />
</template>
