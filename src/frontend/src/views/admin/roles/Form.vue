<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { toTypedSchema } from "@vee-validate/yup";
import { object, string } from "yup";
import { useForm } from "vee-validate";
import { Role } from "@/models/role";

const { t } = useI18n();

const props = defineProps<{
  role: Role | null;
  pageTitle: string;
  loading: boolean;
  routeName: RouteRecordNameGeneric;
}>();

const schema = toTypedSchema(
  object({
    name: string().required(t("admin.roles.fields.name.required")),
    description: string().required(
      t("admin.roles.fields.description.required"),
    ),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<Role>({
  validationSchema: schema,
});

const readOnly: Ref<boolean> = ref(props.routeName === "role-view");

watch(
  () => props.role,
  (roleName) => {
    if (roleName) {
      resetForm({ values: roleName });
    }
  },
  {
    immediate: true,
    deep: true,
  },
);

const [id] = defineField("id");
const [name, nameAttrs] = defineField("name");
const [description, descriptionAttrs] = defineField("description");

const emit = defineEmits<{
  (e: "submit", values: Role): void;
}>();

const submit = handleSubmit((values: Role) => {
  emit("submit", values);
});
</script>

<template>
  <v-card elevation="6" class="mt-4" :disabled="loading">
    <v-card-title>
      <toolbar
        color="secondary"
        :title="pageTitle"
        :button="{
          icon: 'bx-chevron-left',
          text: t('shared.back'),
          to: { name: 'role-list' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-form :disabled="readOnly">
        <v-row v-show="routeName !== 'role-create'">
          <v-col cols="12" md="3">
            <label
              for="id"
              class="form-label"
              v-text="t('admin.roles.fields.id.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="id"
              variant="outlined"
              :disabled="true"
              :label="t('admin.roles.fields.id.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="name"
              class="form-label"
              v-text="t('admin.roles.fields.name.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="name"
              v-bind="nameAttrs"
              variant="outlined"
              :label="t('admin.roles.fields.name.title')"
              :error-messages="errors.name"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="description"
              v-text="t('admin.roles.fields.description.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="description"
              v-bind="descriptionAttrs"
              variant="outlined"
              :label="t('admin.roles.fields.description.title')"
              :error-messages="errors.description"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <v-btn
              color="primary"
              variant="flat"
              size="small"
              prepend-icon="bx-save"
              v-if="!readOnly"
              @click="submit"
            >
              {{ t("shared.save") }}
            </v-btn>
            <v-btn
              color="info"
              size="small"
              variant="flat"
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
