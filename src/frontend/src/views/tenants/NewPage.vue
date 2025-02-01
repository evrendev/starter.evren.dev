<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useForm } from "vee-validate";
import { object, string, boolean, date } from "yup";
import BaseBreadcrumb from "@/components/shared/BaseBreadcrumb.vue";
import { useTenantStore } from "@/stores/tenants";
import { useAppStore } from "@/stores/app";
import { storeToRefs } from "pinia";

const { t } = useI18n();
const router = useRouter();
const tenantStore = useTenantStore();
const appStore = useAppStore();
const { loading } = storeToRefs(appStore);

const tab = ref(0);

const breadcrumbs = ref([
  {
    title: t("admin.tenants.title"),
    disabled: false,
    href: "/admin/tenants"
  },
  {
    title: t("admin.tenants.new"),
    disabled: true,
    href: "#"
  }
]);

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

const { defineField, handleSubmit, resetForm } = useForm({
  validationSchema: schema,
  initialValues: {
    name: "",
    adminEmail: "",
    connectionString: "",
    host: "",
    isActive: true,
    validUntil: null,
    description: ""
  }
});

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
    await tenantStore.create(values);
    router.push("/admin/tenants");
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoader(false);
  }
});
</script>

<template>
  <base-breadcrumb :title="t('admin.tenants.new')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
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
              <v-btn color="error" variant="outlined" :disabled="loading" @click="resetForm" prepend-icon="$refresh">
                {{ t("common.reset") }}
              </v-btn>
              <v-btn color="primary" type="submit" :loading="loading" prepend-icon="$contentSave" class="ml-2">
                {{ t("common.save") }}
              </v-btn>
            </v-col>
          </v-row>
        </v-form>
      </v-card>
    </v-col>
  </v-row>
</template>
