<script setup>
import { ref, nextTick } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useForm } from "vee-validate";
import { object, string } from "yup";
import { useFountainDonationStore } from "@/stores/fountainDonations";
import { useAppStore } from "@/stores/app";
import { storeToRefs } from "pinia";

const props = defineProps({
  initialData: {
    type: Object,
    default: () => null
  },
  isEdit: {
    type: Boolean,
    default: false
  }
});

const { t } = useI18n();
const router = useRouter();
const fountainDonationStore = useFountainDonationStore();
const appStore = useAppStore();
const { loading } = storeToRefs(appStore);

const validationSchema = object().shape({
  contact: string()
    .required(t("admin.donations.fountains.validation.contact.required"))
    .max(100, t("admin.donations.fountains.validation.contact.maxLength")),
  phone: string()
    .required(t("admin.donations.fountains.validation.phone.required"))
    .max(100, t("admin.donations.fountains.validation.phone.maxLength")),
  banner: string()
    .required(t("admin.donations.fountains.validation.banner.required"))
    .max(1000, t("admin.donations.fountains.validation.banner.maxLength")),
  projectCode: string()
    .oneOf(["BKS", "BGS", "AKI", "AGI"], t("admin.donations.fountains.validation.projectCode.invalid"))
    .required(t("admin.donations.fountains.validation.projectCode.required")),
  creationDate: string()
    .required(t("admin.donations.fountains.validation.creationDate.required"))
    .matches(/^\d{4}-\d{2}-\d{2}$/, t("admin.donations.fountains.validation.creationDate.invalid"))
});

const projectCodes = ref([
  { title: t("admin.donations.fountains.projectCodes.bks"), value: "BKS" },
  { title: t("admin.donations.fountains.projectCodes.bgs"), value: "BGS" },
  { title: t("admin.donations.fountains.projectCodes.aki"), value: "AKI" },
  { title: t("admin.donations.fountains.projectCodes.agi"), value: "AGI" }
]);

const initialValues = {
  contact: props.isEdit ? props.initialData.contact : "",
  phone: props.isEdit ? props.initialData.phone : "",
  banner: props.isEdit ? props.initialData.banner : "",
  projectCode: props.isEdit ? props.initialData.projectCode : "BKS",
  creationDate: props.isEdit ? props.initialData.creationDate.pluginDate : new Date().toISOString().split("T")[0]
};

const { defineField, handleSubmit, resetForm } = useForm({
  validationSchema,
  initialValues
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const [contact, contactProps] = defineField("contact", vuetifyConfig);
const [phone, phoneProps] = defineField("phone", vuetifyConfig);
const [banner, bannerProps] = defineField("banner", vuetifyConfig);
const [creationDate, creationDateProps] = defineField("creationDate", vuetifyConfig);
const [projectCode, projectCodeProps] = defineField("projectCode", vuetifyConfig);

const onSubmit = handleSubmit(async (values) => {
  try {
    appStore.setLoading(true);

    if (props.isEdit) {
      await fountainDonationStore.update(props.initialData.id, {
        ...values
      });
    } else {
      await fountainDonationStore.create({
        ...values
      });
    }
    router.push({ name: "list-fountains" });
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setLoading(false);
  }
});

const handleReset = async () => {
  resetForm({ values: initialValues });
  await nextTick();
};
</script>

<template>
  <v-card class="pa-6">
    <v-form @submit="onSubmit">
      <v-row class="mt-2">
        <v-col cols="12" md="3">
          <v-text-field
            v-model="contact"
            v-bind="contactProps"
            :label="t('admin.donations.fountains.fields.contact')"
            :max-length="100"
            density="compact"
            variant="outlined"
            hide-details="auto"
          />
        </v-col>
        <v-col cols="12" md="3">
          <v-text-field
            v-model="phone"
            v-bind="phoneProps"
            :label="t('admin.donations.fountains.fields.phone')"
            :max-length="100"
            density="compact"
            variant="outlined"
            hide-details="auto"
          />
        </v-col>
        <v-col cols="12" md="3">
          <v-text-field
            v-model="creationDate"
            v-bind="creationDateProps"
            :label="t('admin.donations.fountains.fields.creationDate')"
            type="date"
            density="compact"
            variant="outlined"
            hide-details="auto"
          />
        </v-col>
        <v-col cols="12" md="3">
          <v-select
            v-model="projectCode"
            v-bind="projectCodeProps"
            :items="projectCodes"
            :label="t('admin.donations.fountains.fields.projectCode')"
            density="compact"
            hide-details
            item-title="title"
            item-value="value"
            variant="outlined"
          ></v-select>
        </v-col>
        <v-col cols="12">
          <v-textarea
            v-model="banner"
            v-bind="bannerProps"
            :label="t('admin.donations.fountains.fields.banner')"
            :rows="3"
            :counter="1000"
            :max-length="1000"
            density="compact"
            variant="outlined"
            hide-details="auto"
          />
        </v-col>
      </v-row>

      <v-row class="mt-4">
        <v-col cols="12" class="d-flex justify-end gap-2">
          <v-btn color="error" :disabled="loading" @click="handleReset" prepend-icon="$refresh">
            {{ t("common.reset") }}
          </v-btn>
          <v-btn color="primary" type="submit" :loading="loading" :prepend-icon="isEdit ? '$pencil' : '$contentSave'" class="ml-2">
            {{ isEdit ? t("common.update") : t("common.save") }}
          </v-btn>
        </v-col>
      </v-row>
    </v-form>
  </v-card>
</template>
