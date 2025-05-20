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
const absenceStore = useAbsenceStore();
const customizerStore = useCustomizerStore();
const { theme } = storeToRefs(customizerStore);
const { events } = storeToRefs(absenceStore);

const localeMap = {
  tr: "tr-TR",
  en: "en-US",
  de: "de-DE"
};

const viewMonthGrid = createViewMonthGrid();
const viewMonthAgenda = createViewMonthAgenda();

const calendarApp = createCalendar({
  views: [viewMonthGrid, viewMonthAgenda],
  defaultView: viewMonthGrid.name,
  firstDayOfWeek: 1,
  calendars: {
    absence: {
      label: "Absence",
      colorName: "absence",
      lightColors: {
        main: "#d0b316",
        container: "#fff5aa",
        onContainer: "#594800"
      },
      darkColors: {
        main: "#fff5c0",
        onContainer: "#fff5de",
        container: "#a29742"
      }
    },
    sick: {
      label: "Sick",
      colorName: "sick",
      lightColors: {
        main: "#f91c45",
        container: "#ffd2dc",
        onContainer: "#59000d"
      },
      darkColors: {
        main: "#ffc0cc",
        onContainer: "#ffdee6",
        container: "#a24258"
      }
    }
  },
  dayBoundaries: {
    start: "08:00",
    end: "18:00"
  },
  showWeekNumbers: true,
  isResponsive: true,
  isDark: theme.value === "dark",
  events: events.value,
  locale: localeMap[LocaleHelper.currentLocale] || "de-DE"
});

onMounted(async () => {
  await absenceStore.getEvents();
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
  calendarApp.events = events.value;
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
  height: 800px;
  max-height: 90vh;

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
