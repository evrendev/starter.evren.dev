<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { toTypedSchema } from "@vee-validate/yup";
import { object, boolean, string } from "yup";
import { useForm } from "vee-validate";
import { Tenant } from "@/models/tenant";

const { t } = useI18n();

const props = defineProps<{
  tenant: Tenant | null;
  pageTitle: string;
  loading: boolean;
  routeName: RouteRecordNameGeneric;
}>();

const schema = toTypedSchema(
  object({
    id: string().required(t("admin.tenants.fields.id.required")),
    name: string().required(t("admin.tenants.fields.name.required")),
    adminEmail: string()
      .email(t("admin.tenants.fields.adminEmail.invalid"))
      .required(t("admin.tenants.fields.adminEmail.required")),
    isActive: boolean()
      .required(t("admin.tenants.fields.isActive.required"))
      .oneOf([true, false], t("admin.tenants.fields.isActive.invalid")),
    validUpto: string().required(t("admin.tenants.fields.validUpto.required")),
    issuer: string().notRequired(),
    connectionString: string().notRequired(),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<Tenant>({
  validationSchema: schema,
});

const isActiveOptions = computed(() => [
  { value: true, text: t("shared.options.isActive.true") },
  { value: false, text: t("shared.options.isActive.false") },
]);

const readOnly: Ref<boolean> = ref(props.routeName === "tenant-view");

watch(
  () => props.tenant,
  (tenantName) => {
    if (tenantName) {
      resetForm({ values: tenantName });
    }
  },
  {
    immediate: true,
    deep: true,
  },
);

const [id, idAttrs] = defineField("id");
const [isActive, isActiveAttrs] = defineField("isActive");
const [issuer, issuerAttrs] = defineField("issuer");
const [name, nameAttrs] = defineField("name");
const [adminEmail, adminEmailAttrs] = defineField("adminEmail");
const [validUpto, validUptoAttrs] = defineField("validUpto");
const [connectionString, connectionStringAttrs] =
  defineField("connectionString");

const emit = defineEmits<{
  (e: "submit", values: Tenant): void;
}>();

const submit = handleSubmit((values: Tenant) => {
  emit("submit", values);
});
</script>

<template>
  <v-card elevation="6" class="mt-4" :disabled="loading">
    <v-card-title>
      <toolbar
        :title="pageTitle"
        :button="{
          icon: 'bx-chevron-left',
          text: t('shared.back'),
          to: { name: 'tenant-list' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-form :disabled="readOnly">
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="id"
              class="form-label"
              v-text="t('admin.tenants.fields.id.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="id"
              v-bind="idAttrs"
              variant="outlined"
              :label="t('admin.tenants.fields.id.title')"
              :error-messages="errors.id"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="name"
              class="form-label"
              v-text="t('admin.tenants.fields.name.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="name"
              v-bind="nameAttrs"
              variant="outlined"
              :label="t('admin.tenants.fields.name.title')"
              :error-messages="errors.name"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="adminEmail"
              v-text="t('admin.tenants.fields.adminEmail.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="adminEmail"
              v-bind="adminEmailAttrs"
              variant="outlined"
              :label="t('admin.tenants.fields.adminEmail.title')"
              :error-messages="errors.adminEmail"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="connectionString"
              v-text="t('admin.tenants.fields.connectionString.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-textarea
              v-model="connectionString"
              v-bind="connectionStringAttrs"
              variant="outlined"
              clearable
              :label="t('admin.tenants.fields.connectionString.title')"
              :error-messages="errors.connectionString"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="issuer"
              v-text="t('admin.tenants.fields.issuer.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="issuer"
              v-bind="issuerAttrs"
              variant="outlined"
              :label="t('admin.tenants.fields.issuer.title')"
              :error-messages="errors.issuer"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="isActive"
              v-text="t('admin.tenants.fields.isActive.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-select
              v-model="isActive"
              v-bind="isActiveAttrs"
              item-text="value"
              item-title="text"
              variant="outlined"
              hide-details
              :label="t('admin.tenants.fields.isActive.title')"
              :items="isActiveOptions"
              :error-messages="errors.isActive"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="validUpto"
              v-text="t('admin.tenants.fields.validUpto.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="validUpto"
              v-bind="validUptoAttrs"
              type="date"
              variant="outlined"
              hide-details
              :label="t('admin.tenants.fields.validUpto.title')"
              :error-messages="errors.validUpto"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <v-btn
              color="primary"
              variant="tonal"
              size="small"
              prepend-icon="bx-save"
              v-if="!readOnly"
              @click="submit"
            >
              {{ t("shared.save") }}
            </v-btn>
            <v-btn
              color="warning"
              size="small"
              prepend-icon="bx-lock-open-alt"
              v-if="readOnly"
              @click="readOnly = false"
            >
              {{ t("shared.enableEdit") }}
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
  </v-card>
</template>

<style scoped type="scss">
:deep(label) {
  &.form-label {
    font-weight: 600;

    &::after {
      content: ":";
    }
  }
}
</style>
