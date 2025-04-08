<script setup>
import { watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useForm } from "vee-validate";
import { object, string, boolean, date } from "yup";
import { useTenantStore, useAppStore } from "@/stores";
import { NavigationMenu } from "./";

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
    appStore.setLoading(true);
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
    appStore.setLoading(false);
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
  <v-row class="form-container">
    <v-col xs="12" sm="8" md="9" class="order-2 order-sm-1">
      <!-- Tenant Information Card -->
      <v-card>
        <v-toolbar color="primary" id="tenant-information">
          <v-toolbar-title :text="t('admin.tenants.helpers.information')" dark />
          <v-btn icon>
            <v-icon icon="$informationBox" />
          </v-btn>
        </v-toolbar>
        <v-row class="pa-4 mt-2">
          <v-col cols="12" md="4">
            <v-text-field
              v-model="name"
              v-bind="nameProps"
              :label="t('admin.tenants.fields.name')"
              density="compact"
              variant="outlined"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field
              v-model="adminEmail"
              v-bind="adminEmailProps"
              :label="t('admin.tenants.fields.adminEmail')"
              density="compact"
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
              density="compact"
              variant="outlined"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12">
            <v-textarea
              v-model="description"
              v-bind="descriptionProps"
              :label="t('admin.tenants.fields.description')"
              density="compact"
              variant="outlined"
              hide-details="auto"
              rows="3"
            />
          </v-col>
        </v-row>
      </v-card>

      <!-- Additional Parameters Card -->
      <v-card class="mt-4">
        <v-toolbar color="primary" id="tenant-parameters">
          <v-toolbar-title :text="t('admin.tenants.helpers.connection')" dark />
          <v-btn icon>
            <v-icon icon="$informationBox" />
          </v-btn>
        </v-toolbar>
        <v-row class="pa-4 mt-2">
          <v-col cols="12" md="6">
            <v-text-field
              v-model="connectionString"
              v-bind="connectionStringProps"
              :label="t('admin.tenants.fields.connectionString')"
              density="compact"
              variant="outlined"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field
              v-model="host"
              v-bind="hostProps"
              :label="t('admin.tenants.fields.host')"
              density="compact"
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
      </v-card>
    </v-col>

    <!-- Navigation Menu -->
    <navigation-menu :isEdit="isEdit" @reset="handleReset" @submit="onSubmit" class="navigation-container order-1 order-sm-2" />
  </v-row>
</template>

<style lang="scss">
.navigation-container {
  position: sticky;
  position: -webkit-sticky;
  top: 80px;
  z-index: 100;
  height: fit-content;
}
</style>
