<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useForm } from "vee-validate";
import { date, object, string } from "yup";

const props = defineProps({
  event: {
    type: Object,
    default: () => null
  },
  hasCreatePermission: {
    type: Boolean,
    required: true,
    default: false
  },
  hasUpdatePermission: {
    type: Boolean,
    required: true,
    default: false
  },
  hasDeletePermission: {
    type: Boolean,
    required: true,
    default: false
  }
});

const { t } = useI18n();

const calendars = ref([
  { title: t("admin.absences.calendars.holiday"), value: "holiday" },
  { title: t("admin.absences.calendars.ill"), value: "ill" },
  { title: t("admin.absences.calendars.appointment"), value: "appointment" },
  { title: t("admin.absences.calendars.travel"), value: "travel" },
  { title: t("admin.absences.calendars.school"), value: "school" },
  { title: t("admin.absences.calendars.medicalAppointment"), value: "medicalAppointment" },
  { title: t("admin.absences.calendars.visit"), value: "visit" }
]);

const locations = ref(["Delmenhorst", "Izmir"]);

const today = new Date().toISOString().slice(0, 10).replace(/-/g, "-");
const tomorrow = new Date(new Date().setDate(new Date().getDate() + 1)).toISOString().slice(0, 10).replace(/-/g, "-");
const defaultValues = {
  calendarId: props.event?.calendarId || "holiday",
  employee: props.event?.employee || "",
  description: props.event?.description || "",
  start: props.event?.start || today,
  end: props.event?.end || tomorrow,
  location: props.event?.location || locations.value[0]
};

const schema = object().shape({
  employee: string().required(t("admin.absences.validation.employee.required")).max(100, t("admin.absences.validation.employee.maxLength")),
  calendarId: string()
    .required(t("admin.absences.validation.calendarId.required"))
    .oneOf(
      calendars.value.map((item) => item.value),
      t("admin.absences.validation.calendarId.invalid")
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
const [calendarId, calendarIdProps] = defineField("calendarId", vuetifyConfig);
const [location, locationProps] = defineField("location", vuetifyConfig);
const [start, startProps] = defineField("start", vuetifyConfig);
const [end, endProps] = defineField("end", vuetifyConfig);
const [description, descriptionProps] = defineField("description", vuetifyConfig);

const emits = defineEmits(["closeEventDialog", "saveEvent", "updateEvent", "deleteEvent"]);

const onSubmit = handleSubmit((values) => {
  if (props.event) {
    values.id = props.event.id;
    emits("updateEvent", values);
  } else {
    emits("saveEvent", values);
  }
});

const handleDelete = () => {
  emits("deleteEvent", props.event.id);
};
</script>
<template>
  <v-form @submit="onSubmit">
    <v-row class="mt-2">
      <v-col cols="12" md="8">
        <v-text-field
          v-model="employee"
          v-bind="nameProps"
          :disabled="event && !hasUpdatePermission"
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
          :disabled="event && !hasUpdatePermission"
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
          v-model="calendarId"
          v-bind="calendarIdProps"
          :items="calendars"
          :disabled="event && !hasUpdatePermission"
          :label="t('admin.absences.fields.calendarId')"
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
          :disabled="event && !hasUpdatePermission"
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
          :disabled="event && !hasUpdatePermission"
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
          :disabled="event && !hasUpdatePermission"
          :label="t('admin.absences.fields.description')"
          density="compact"
          variant="outlined"
          hide-details="auto"
        />
      </v-col>
    </v-row>

    <v-row class="mt-4">
      <v-col cols="12" class="d-flex justify-end gap-2">
        <v-btn v-if="event && hasDeletePermission" color="error" prepend-icon="$trashCan" class="ml-2" @click="handleDelete">
          {{ t("common.delete") }}
        </v-btn>

        <v-btn v-if="!event && hasCreatePermission" color="primary" type="submit" prepend-icon="$contentSave" class="ml-2">
          {{ t("common.save") }}
        </v-btn>

        <v-btn v-if="event && hasUpdatePermission" color="primary" type="submit" prepend-icon="$pencil" class="ml-2">
          {{ t("common.update") }}
        </v-btn>
      </v-col>
    </v-row>
  </v-form>
</template>
