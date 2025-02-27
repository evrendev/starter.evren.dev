<script setup>
import { watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useForm } from "vee-validate";
import { object, string } from "yup";
import { useRoleStore } from "@/stores/roles";
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
const roleStore = useRoleStore();
const appStore = useAppStore();
const { loading } = storeToRefs(appStore);

const schema = object().shape({
  name: string().required(t("admin.roles.validation.name.required")).max(200, t("admin.roles.validation.name.maxLength")),
  description: string().nullable().max(500, t("admin.roles.validation.description.maxLength"))
});

const defaultValues = {
  name: "",
  description: ""
};

const { defineField, handleSubmit, resetForm, setValues } = useForm({
  validationSchema: schema,
  initialValues: defaultValues
});

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
const [description, descriptionProps] = defineField("description", vuetifyConfig);

const onSubmit = handleSubmit(async (values) => {
  try {
    appStore.setPageLoading(true);
    const submitData = {
      ...values,
      validUntil: values.validUntil ? new Date(values.validUntil).toISOString() : null
    };

    if (props.isEdit) {
      await roleStore.update(props.initialData.id, submitData);
    } else {
      await roleStore.create(submitData);
    }

    router.push({ name: "list-roles" });
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoading(false);
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
      <v-row class="mt-2">
        <v-col cols="12" md="4">
          <v-text-field
            v-model="name"
            v-bind="nameProps"
            :label="t('admin.roles.fields.name')"
            density="comfortable"
            variant="outlined"
            hide-details="auto"
          />
        </v-col>
        <v-col cols="8">
          <v-text-field
            v-model="description"
            v-bind="descriptionProps"
            :label="t('admin.roles.fields.description')"
            density="comfortable"
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
