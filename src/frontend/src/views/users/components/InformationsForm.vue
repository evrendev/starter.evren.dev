<script setup>
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { useForm } from "vee-validate";
import { object, string, ref as yupRef } from "yup";
import { usePredefinedValuesStore } from "@/stores";
import { storeToRefs } from "pinia";
import config from "@/config";

const { t } = useI18n();

const predefinedValues = usePredefinedValuesStore();
const { genders, languages, loading } = storeToRefs(predefinedValues);

const showPassword = ref(false);

defineProps({
  initialData: {
    type: Object,
    default: () => null
  },
  isEdit: {
    type: Boolean,
    default: false
  }
});

const defaultValues = {
  tenantId: null,
  gender: "none",
  email: "",
  password: "",
  firstName: "",
  lastName: "",
  jobTitle: "",
  language: config.defaultLocale
};

const schema = object().shape({
  gender: string().required(t("admin.users.validation.gender.required")),
  email: string().required(t("admin.users.validation.email.required")).email(t("admin.users.validation.email.invalid")),
  firstName: string().required(t("admin.users.validation.firstName.required")).max(100, t("admin.users.validation.firstName.maxLength")),
  lastName: string().required(t("admin.users.validation.lastName.required")).max(100, t("admin.users.validation.lastName.maxLength")),
  password: string()
    .required(t("admin.users.validation.password.required"))
    .min(8, t("admin.users.validation.password.minLength"))
    .matches(/^[A-Za-z0-9!@#$%^&*()_+|~\-={}[\]:";<>?,./]+$/, t("admin.users.validation.password.special")),
  confirmPassword: string()
    .required(t("admin.users.validation.confirmPassword.required"))
    .oneOf([yupRef("password")], t("admin.users.validation.confirmPassword.match")),
  jobTitle: string().nullable().max(500, t("admin.users.validation.jobTitle.maxLength")),
  language: string().required()
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const { validate, values, defineField, resetForm } = useForm({
  validationSchema: schema,
  initialValues: defaultValues
});

defineExpose({ validateForm, values, resetValues });

const [gender, genderProps] = defineField("gender", vuetifyConfig);
const [email, emailProps] = defineField("email", vuetifyConfig);
const [password, passwordProps] = defineField("password", vuetifyConfig);
const [confirmPassword, confirmPasswordProps] = defineField("confirmPassword", vuetifyConfig);
const [firstName, firstNameProps] = defineField("firstName", vuetifyConfig);
const [lastName, lastNameProps] = defineField("lastName", vuetifyConfig);
const [jobTitle, jobTitleProps] = defineField("jobTitle", vuetifyConfig);
const [language, languageProps] = defineField("language", vuetifyConfig);

onMounted(async () => {
  await predefinedValues.get();
});

async function validateForm() {
  return await validate();
}

function resetValues() {
  resetForm();
}
</script>

<template>
  <v-card>
    <v-toolbar color="primary" id="user-information">
      <v-toolbar-title :text="t('admin.users.helpers.information')" dark />
      <v-btn icon>
        <v-icon icon="$informationBox" />
      </v-btn>
    </v-toolbar>
    <v-row class="pa-4 mt-2">
      <v-col cols="12" md="3">
        <v-select
          v-model="language"
          v-bind="languageProps"
          :items="languages || []"
          :label="$t('admin.users.fields.language')"
          :loading="loading"
          item-title="name"
          item-value="code"
          density="comfortable"
          variant="outlined"
        />
      </v-col>

      <v-col cols="12" md="3">
        <v-select
          v-model="gender"
          v-bind="genderProps"
          :items="genders || []"
          :label="$t('admin.users.fields.gender')"
          :loading="loading"
          density="comfortable"
          variant="outlined"
          item-title="name"
          item-value="code"
        />
      </v-col>

      <v-col cols="12" md="6">
        <v-text-field
          v-model="email"
          v-bind="emailProps"
          :label="$t('admin.users.fields.email')"
          density="comfortable"
          variant="outlined"
          hide-details="auto"
        />
      </v-col>

      <v-col cols="12" md="6">
        <v-text-field
          v-model="firstName"
          v-bind="firstNameProps"
          :label="$t('admin.users.fields.firstName')"
          density="comfortable"
          variant="outlined"
        />
      </v-col>

      <v-col cols="12" md="6">
        <v-text-field
          v-model="lastName"
          v-bind="lastNameProps"
          :label="$t('admin.users.fields.lastName')"
          density="comfortable"
          variant="outlined"
        />
      </v-col>

      <v-col cols="12" md="6">
        <v-text-field
          v-model="password"
          v-bind="passwordProps"
          :label="$t('admin.users.fields.password')"
          :append-icon="showPassword ? '$eye' : '$eyeOff'"
          :type="showPassword ? 'text' : 'password'"
          @click:append="showPassword = !showPassword"
          density="comfortable"
          variant="outlined"
          color="primary"
          hide-details="auto"
          class="pwd-input"
        />
      </v-col>

      <v-col cols="12" md="6">
        <v-text-field
          v-model="confirmPassword"
          v-bind="confirmPasswordProps"
          :label="$t('admin.users.fields.confirmPassword')"
          density="comfortable"
          variant="outlined"
          type="password"
          hide-details="auto"
        />
      </v-col>

      <v-col cols="12">
        <v-text-field
          v-model="jobTitle"
          v-bind="jobTitleProps"
          :label="t('admin.users.fields.jobTitle')"
          density="comfortable"
          variant="outlined"
          hide-details="auto"
        />
      </v-col>
    </v-row>
  </v-card>
</template>

<style lang="scss">
.pwd-input {
  position: relative;
  .v-input__append {
    position: absolute;
    right: 10px;
    top: 50%;
    transform: translateY(-50%);
  }
}
</style>
