<script setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useForm } from "vee-validate";
import { object, string, boolean, date } from "yup";
import { useTenantStore, useAppStore } from "@/stores";
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
const tenantStore = useTenantStore();
const appStore = useAppStore();
const { loading } = storeToRefs(appStore);

const tab = ref(0);

const schema = object().shape({
  name: string().required(t("admin.tenants.validation.name.required")).max(200, t("admin.tenants.validation.name.maxLength")),
  adminEmail: string().required(t("admin.tenants.validation.adminEmail.required")).email(t("admin.tenants.validation.adminEmail.invalid")),
  connectionString: string().nullable(),
  host: string().nullable(),
  isActive: boolean().default(true),
  validUntil: date()
    .required(t("admin.tenants.validation.validUntil.required"))
    .min(new Date(), t("admin.tenants.validation.validUntil.future")),
  description: string().nullable().max(500, t("admin.tenants.validation.description.maxLength"))
});

const defaultValues = {
  name: "",
  adminEmail: "",
  connectionString: "",
  host: "",
  isActive: true,
  validUntil: null,
  description: ""
};

const { defineField, handleSubmit, resetForm, setValues } = useForm({
  validationSchema: schema,
  initialValues: defaultValues
});

// Watch for changes in initialData and update form values
watch(
  () => props.initialData,
  (newValue) => {
    if (newValue) {
      const formattedData = {
        ...defaultValues,
        ...newValue,
        validUntil: newValue.validUntil?.pluginDate || null
      };
      setValues(formattedData);
    }
  },
  { immediate: true, deep: true }
);

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const [name, nameProps] = defineField("name", vuetifyConfig);
const [adminEmail, adminEmailProps] = defineField("adminEmail", vuetifyConfig);
const [connectionString, connectionStringProps] = defineField("connectionString", vuetifyConfig);
const [host, hostProps] = defineField("host", vuetifyConfig);
const [isActive, isActiveProps] = defineField("isActive", vuetifyConfig);
const [validUntil, validUntilProps] = defineField("validUntil", vuetifyConfig);
const [description, descriptionProps] = defineField("description", vuetifyConfig);

const onSubmit = handleSubmit(async (values) => {
  try {
    appStore.setPageLoader(true);
    const submitData = {
      ...values,
      validUntil: values.validUntil ? new Date(values.validUntil).toISOString() : null
    };

    if (props.isEdit) {
      await tenantStore.update(props.initialData.id, submitData);
    } else {
      await tenantStore.create(submitData);
    }

    router.push({ name: "list-tenants" });
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoader(false);
  }
});

const handleReset = () => {
  if (props.isEdit && props.initialData) {
    setValues({
      ...defaultValues,
      ...props.initialData,
      validUntil: props.initialData.validUntil?.pluginDate || null
    });
  } else {
    resetForm();
  }
};
</script>

<template>
  <v-card class="pa-6">
    <v-form @submit="onSubmit">
      <v-tabs v-model="tab" color="primary" align-tabs="start">
        <v-tab :value="0">{{ t("common.basicInformation") }}</v-tab>
        <v-tab :value="1">{{ t("common.additionalParameters") }}</v-tab>
      </v-tabs>

      <v-window v-model="tab">
        <v-window-item :value="0">
          <v-row class="mt-2">
            <v-col cols="12" md="4">
              <v-text-field
                v-model="name"
                v-bind="nameProps"
                :label="t('admin.tenants.fields.name')"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="12" md="4">
              <v-text-field
                v-model="adminEmail"
                v-bind="adminEmailProps"
                :label="t('admin.tenants.fields.adminEmail')"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="12" md="4">
              <v-text-field
                v-model="validUntil"
                v-bind="validUntilProps"
                :label="t('admin.tenants.fields.validUntil')"
                type="date"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="12">
              <v-textarea
                v-model="description"
                v-bind="descriptionProps"
                :label="t('admin.tenants.fields.description')"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
                rows="3"
              />
            </v-col>
          </v-row>
        </v-window-item>

        <v-window-item :value="1">
          <v-row class="mt-2">
            <v-col cols="12" md="6">
              <v-text-field
                v-model="connectionString"
                v-bind="connectionStringProps"
                :label="t('admin.tenants.fields.connectionString')"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="12" md="4">
              <v-text-field
                v-model="host"
                v-bind="hostProps"
                :label="t('admin.tenants.fields.host')"
                density="comfortable"
                variant="outlined"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="12" md="2">
              <v-switch
                v-model="isActive"
                v-bind="isActiveProps"
                :label="t('admin.tenants.fields.isActive')"
                color="success"
                hide-details="auto"
              />
            </v-col>
          </v-row>
        </v-window-item>
      </v-window>

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
