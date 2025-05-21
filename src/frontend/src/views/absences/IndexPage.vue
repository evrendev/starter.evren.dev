<script setup>
import { shallowRef, ref, onMounted, nextTick } from "vue";
import { useI18n } from "vue-i18n";
import { Breadcrumb } from "@/components/forms";
import { useAbsenceStore, useAuthStore } from "@/stores";
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

const authStore = useAuthStore();
const { permissions } = storeToRefs(authStore);
const ABSENCE_PERMISSIONS = {
  Create: "Absences.Create",
  Update: "Absences.Edit",
  Delete: "Absences.Delete"
};

const hasCreatePermission = permissions.value.some((p) => p === ABSENCE_PERMISSIONS.Create);
const hasUpdatePermission = permissions.value.some((p) => p === ABSENCE_PERMISSIONS.Update);
const hasDeletePermission = permissions.value.some((p) => p === ABSENCE_PERMISSIONS.Delete);

const loading = ref(true);
const event = ref(null);
const showEventDialog = ref(false);
const absenceStore = useAbsenceStore();
const { events } = storeToRefs(absenceStore);

onMounted(async () => {
  await absenceStore.getEvents();
  loading.value = false;
});

const saveEvent = async (item) => {
  loading.value = true;
  const id = await absenceStore.save(item);
  item.id = id;
  updateEventInformationOnDOM(item);
  loading.value = false;
  showEventDialog.value = false;
};

const updateEvent = async (item) => {
  loading.value = true;
  await absenceStore.update(item.id, item);
  updateEventInformationOnDOM(item);
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

const updateEventInformationOnDOM = async (item) => {
  await nextTick();
  setTimeout(() => {
    const el = document.querySelector(`[data-event-id="${item.id}"]`);
    if (el) {
      const titleElement = el.querySelector(".sx__month-grid-event-title");
      if (titleElement) {
        titleElement.textContent = `${item.employee} - ${item.location} - ${item.description}`;
      }
    }
  }, 1000);
};
</script>

<template>
  <breadcrumb :title="t('admin.absences.title')" :breadcrumbs="breadcrumbs" />
  <calendar-app
    v-if="!loading"
    :loading="loading"
    :has-create-permission="hasCreatePermission"
    :events="events"
    @show-event-dialog="showEvent"
  />
  <event-dialog
    :showEventDialog="showEventDialog"
    :event="event"
    :has-create-permission="hasCreatePermission"
    :has-update-permission="hasUpdatePermission"
    :has-delete-permission="hasDeletePermission"
    @save-event="saveEvent"
    @update-event="updateEvent"
    @delete-event="deleteEvent"
    @close-dialog="closeDialog"
  />
</template>
