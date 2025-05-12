<script setup>
import { shallowRef, ref, onMounted, watch } from "vue";
import { useI18n } from "vue-i18n";
import { Breadcrumb } from "@/components/forms";
import { useAbsenceStore, useCustomizerStore } from "@/stores";
import { storeToRefs } from "pinia";
import { LocaleHelper } from "@/utils/helpers";
import { ScheduleXCalendar } from "@schedule-x/vue";
import { createCalendar, createViewMonthAgenda, createViewMonthGrid } from "@schedule-x/calendar";
import "@schedule-x/theme-default/dist/index.css";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.absences.title"),
    disabled: true,
    href: "#"
  }
]);

const loading = ref(false);
const showAbsenceForm = ref(false);
const items = ref([]);
const absenceStore = useAbsenceStore();
const customizerStore = useCustomizerStore();
const { theme } = storeToRefs(customizerStore);

const localeMap = {
  tr: "tr-TR",
  en: "en-US",
  de: "de-DE"
};

onMounted(() => {
  absenceStore.getItems();
  items.value = absenceStore.items;
});

const viewMonthGrid = createViewMonthGrid();
const viewMonthAgenda = createViewMonthAgenda();

const calendarApp = createCalendar({
  views: [viewMonthGrid, viewMonthAgenda],
  defaultView: viewMonthGrid.name,
  events: items.value,
  firstDayOfWeek: 1,
  showWeekNumbers: true,
  isResponsive: true,
  isDark: theme.value === "dark",
  locale: localeMap[LocaleHelper.currentLocale] || "de-DE"
});

watch(
  () => [theme.value, LocaleHelper.currentLocale],
  ([themeValue, localeValue]) => {
    calendarApp.setLocale = localeMap[localeValue] || "de-DE";
    calendarApp.setTheme(themeValue);
  },
  { immediate: true }
);

const openAbsenceForm = () => {
  showAbsenceForm.value = true;
};
</script>

<template>
  <breadcrumb :title="t('admin.absences.title')" :breadcrumbs="breadcrumbs" />

  <v-row>
    <v-col cols="12">
      <schedule-x-calendar :calendar-app="calendarApp">
        <template #headerContentRightAppend>
          <v-btn color="primary" size="small" :loading="loading" @click="openAbsenceForm" prepend-icon="$plusCircle">{{
            t("common.new")
          }}</v-btn>
        </template>
      </schedule-x-calendar>
    </v-col>
  </v-row>

  <v-dialog :model-value="showAbsenceForm" max-width="800" class="dialog-colored-title">
    <v-card>
      <v-card-title class="text-h5">
        {{ t("admin.absences.new") }}
      </v-card-title>
      <v-card-text> TEST </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" @click="console.log(true)" prepend-icon="$contentSave" variant="elevated">
          {{ t("common.save") }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style lang="scss" scoped>
:deep(.sx-vue-calendar-wrapper) {
  .sx__calendar-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .sx__calendar-header-content {
      .sx__view-selection,
      .sx__date-picker-wrapper {
        display: none;
      }

      .sx__today-button {
        padding: 5px 15px;
      }
    }
  }
}
</style>
