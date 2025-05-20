<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useForm } from "vee-validate";
import { date, object, string } from "yup";

defineProps({
  event: {
    type: Object,
    default: () => null
  }
});

const { t } = useI18n();

const calendars = ref([
  { title: t("admin.absences.calendar.absence"), value: "absence" },
  { title: t("admin.absences.calendar.sick"), value: "sick" }
]);

const locations = ref(["Bremen", "Essen"]);

const defaultValues = {
  calendar: calendars.value[0].value,
  employee: "",
  description: "",
  start: new Date().toISOString().slice(0, 10).replace(/-/g, "-"),
  end: new Date(new Date().setDate(new Date().getDate() + 7)).toISOString().slice(0, 10).replace(/-/g, "-"),
  location: locations.value[0]
};

const schema = object().shape({
  employee: string().required(t("admin.absences.validation.employee.required")).max(100, t("admin.absences.validation.employee.maxLength")),
  calendar: string()
    .required(t("admin.absences.validation.calendar.required"))
    .oneOf(
      calendars.value.map((item) => item.value),
      t("admin.absences.validation.calendar.invalid")
    ),
  location: string()
    .required(t("admin.absences.validation.location.required"))
    .oneOf(locations.value, t("admin.absences.validation.location.invalid")),
  description: string().nullable().max(1000, t("admin.absences.validation.description.maxLength")),
  start: date().required(t("admin.absences.validation.start.required")).typeError(t("admin.absences.validation.start.typeError")),
  end: date()
    .required(t("admin.absences.validation.end.required"))
    .typeError(t("admin.absences.validation.end.typeError"))
    .test("is-greater", t("admin.absences.validation.end.isGreater"), function (value) {
      const { start } = this.parent;
      return value >= start;
    })
});

const { defineField, handleSubmit } = useForm({
  validationSchema: schema,
  initialValues: defaultValues
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const [employee, nameProps] = defineField("employee", vuetifyConfig);
const [calendar, calendarProps] = defineField("calendar", vuetifyConfig);
const [location, locationProps] = defineField("location", vuetifyConfig);
const [start, startProps] = defineField("start", vuetifyConfig);
const [end, endProps] = defineField("end", vuetifyConfig);
const [description, descriptionProps] = defineField("description", vuetifyConfig);

const emits = defineEmits(["closeEventDialog", "saveEvent"]);

const onSubmit = handleSubmit((values) => {
  emits("saveEvent", values);
});
</script>
<template>
  <v-form @submit="onSubmit">
    <v-row class="mt-2">
      <v-col cols="12" md="8">
        <v-text-field
          v-model="employee"
          v-bind="nameProps"
          :label="t('admin.absences.fields.employee')"
          density="compact"
          variant="outlined"
          hide-details="auto"
        />
      </v-col>
      <v-col cols="12" md="4">
        <v-select
          v-model="location"
          v-bind="locationProps"
          :items="locations"
          :label="t('admin.absences.fields.location')"
          density="compact"
          hide-details
          item-title="title"
          item-value="value"
          variant="outlined"
        />
      </v-col>
    </v-row>
    <v-row class="mt-2">
      <v-col cols="12" md="4">
        <v-select
          v-model="calendar"
          v-bind="calendarProps"
          :items="calendars"
          :label="t('admin.absences.fields.calendar')"
          density="compact"
          hide-details
          item-title="title"
          item-value="value"
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" md="4">
        <v-text-field
          v-model="start"
          v-bind="startProps"
          :label="t('common.selectDate')"
          density="compact"
          hide-details
          type="date"
          variant="outlined"
        ></v-text-field>
      </v-col>
      <v-col cols="12" md="4">
        <v-text-field
          v-model="end"
          v-bind="endProps"
          :label="t('common.selectDate')"
          density="compact"
          hide-details
          type="date"
          variant="outlined"
        ></v-text-field>
      </v-col>
    </v-row>
    <v-row class="mt-2">
      <v-col cols="12">
        <v-text-field
          v-model="description"
          v-bind="descriptionProps"
          :label="t('admin.absences.fields.description')"
          density="compact"
          variant="outlined"
          hide-details="auto"
        />
      </v-col>
    </v-row>

    <v-row class="mt-4">
      <v-col cols="12" class="d-flex justify-end gap-2">
        <v-btn color="primary" type="submit" prepend-icon="$contentSave" class="ml-2">
          {{ t("common.save") }}
        </v-btn>
      </v-col>
    </v-row>
  </v-form>
</template>
