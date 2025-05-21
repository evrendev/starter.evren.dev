<script setup>
import { onMounted, shallowRef, watch } from "vue";
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
  }
});

const localeMap = {
  tr: "tr-TR",
  en: "en-US",
  de: "de-DE"
};

const calendars = shallowRef({
  holiday: {
    colorName: "holiday",
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
  ill: {
    colorName: "ill",
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
  },
  appontment: {
    colorName: "appointment",
    lightColors: {
      main: "#0072ff",
      container: "#cce0ff",
      onContainer: "#001e6b"
    },
    darkColors: {
      main: "#a3c4ff",
      onContainer: "#cce0ff",
      container: "#0039cb"
    }
  },
  travel: {
    colorName: "travel",
    lightColors: {
      main: "#ff8c00",
      container: "#ffe0b2",
      onContainer: "#4e3b00"
    },
    darkColors: {
      main: "#ffb74d",
      onContainer: "#ffe0b2",
      container: "#ff6f00"
    }
  },
  school: {
    colorName: "school",
    lightColors: {
      main: "#4caf50",
      container: "#c8e6c9",
      onContainer: "#1b5e20"
    },
    darkColors: {
      main: "#a5d6a7",
      onContainer: "#c8e6c9",
      container: "#388e3c"
    }
  },
  medicalAppointment: {
    colorName: "medicalAppointment",
    lightColors: {
      main: "#673ab7",
      container: "#d1c4e9",
      onContainer: "#311b92"
    },
    darkColors: {
      main: "#d1c4e9",
      onContainer: "#d1c4e9",
      container: "#512da8"
    }
  },
  visit: {
    colorName: "visit",
    lightColors: {
      main: "#ff5722",
      container: "#ffccbc",
      onContainer: "#bf360c"
    },
    darkColors: {
      main: "#ff8a65",
      onContainer: "#ffccbc",
      container: "#d84315"
    }
  }
});

const viewMonthGrid = createViewMonthGrid();
const viewMonthAgenda = createViewMonthAgenda();
const calendar = createCalendar({
  views: [viewMonthGrid, viewMonthAgenda],
  defaultView: viewMonthGrid.name,
  firstDayOfWeek: 1,
  calendars: calendars.value,
  dayBoundaries: {
    start: "08:00",
    end: "18:00"
  },
  showWeekNumbers: true,
  isResponsive: true,
  isDark: theme.value === "dark",
  events: props.events,
  locale: localeMap[LocaleHelper.currentLocale] || "de-DE",
  callbacks: {
    onEventClick: (event) => {
      emits("showEventDialog", event);
    }
  }
});

onMounted(() => {
  calendar.events = props.events;
});

watch(
  () => [theme.value, LocaleHelper.currentLocale],
  ([themeValue, localeValue]) => {
    calendar.setLocale = localeMap[localeValue] || "de-DE";
    calendar.setTheme(themeValue);
  },
  { immediate: true }
);

const emits = defineEmits(["showEventDialog"]);
</script>
<template>
  <v-row>
    <v-col cols="12">
      <schedule-x-calendar :calendar-app="calendar">
        <template #headerContentRightAppend>
          <v-btn
            color="primary"
            size="small"
            prepend-icon="$plusCircle"
            :loading="loading"
            @click="$emit('showEventDialog')"
            :text="t('common.new')"
          />
        </template>
      </schedule-x-calendar>
    </v-col>
  </v-row>
  <v-row class="mt-4">
    <v-col cols="12">
      <v-card class="calendar-legend px-4 py-2">
        <div class="text-center">
          <h3 class="text-h4">{{ t("admin.absences.fields.calendarId") }}</h3>
        </div>
        <div class="d-flex flex-row ga-2 mt-2">
          <div v-for="(calendar, key) in calendars" :key="key" class="legend-item ml-2">
            <div
              class="color-circle"
              :style="{
                backgroundColor: theme === 'dark' ? calendar.darkColors.main : calendar.lightColors.main
              }"
            ></div>
            <span class="legend-text">{{ t(`admin.absences.calendars.${calendar.colorName}`) }}</span>
          </div>
        </div>
      </v-card>
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

.calendar-legend {
  .legend-item {
    display: flex;
    align-items: center;
    gap: 2px;

    .color-circle {
      width: 12px;
      height: 12px;
      border-radius: 50%;
      border: 1px solid #000;
    }

    .legend-text {
      font-size: 14px;
      font-weight: 500;
    }
  }
}
</style>
