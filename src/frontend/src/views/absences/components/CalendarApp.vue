<script setup>
import { onMounted, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useCustomizerStore } from "@/stores";
import { storeToRefs } from "pinia";
import { LocaleHelper } from "@/utils/helpers";
import { ScheduleXCalendar } from "@schedule-x/vue";
import { createCalendar, createViewMonthAgenda, createViewMonthGrid } from "@schedule-x/calendar";
import "@schedule-x/theme-default/dist/index.css";

const { t } = useI18n();
const customizerStore = useCustomizerStore();
const { theme } = storeToRefs(customizerStore);

const props = defineProps({
  events: {
    type: Array,
    required: true,
    default: () => []
  },
  loading: {
    type: Boolean,
    required: true,
    default: false
  },
  render: {
    type: Boolean,
    required: true,
    default: false
  }
});

const localeMap = {
  tr: "tr-TR",
  en: "en-US",
  de: "de-DE"
};
const viewMonthGrid = createViewMonthGrid();
const viewMonthAgenda = createViewMonthAgenda();
const calendar = createCalendar({
  views: [viewMonthGrid, viewMonthAgenda],
  defaultView: viewMonthGrid.name,
  firstDayOfWeek: 1,
  calendars: {
    absence: {
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
  events: props.events,
  locale: localeMap[LocaleHelper.currentLocale] || "de-DE"
});

onMounted(() => {
  calendar.events = props.events;
});

watch(
  () => [theme.value, LocaleHelper.currentLocale, props.loading, props.render],
  ([themeValue, localeValue]) => {
    calendar.setLocale = localeMap[localeValue] || "de-DE";
    calendar.setTheme(themeValue);
    calendar.events = props.events;
    if (props.render) calendar.render();
  },
  { immediate: true }
);

defineEmits(["showEventDialog"]);
</script>
<template>
  <v-row>
    <v-col cols="12">
      <schedule-x-calendar :calendar-app="calendar">
        <template #headerContentRightAppend>
          <v-btn color="primary" size="small" :loading="loading" @click="$emit('showEventDialog')" prepend-icon="$plusCircle">{{
            t("common.new")
          }}</v-btn>
        </template>
      </schedule-x-calendar>
    </v-col>
  </v-row>
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
