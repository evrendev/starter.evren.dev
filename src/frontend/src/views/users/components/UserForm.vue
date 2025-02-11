<script setup>
import { ref, onMounted, watch, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useForm } from "vee-validate";
import { object, string } from "yup";
import { useUserStore, usePredefinedValuesStore, useAppStore, useAuthStore } from "@/stores";
import { storeToRefs } from "pinia";
import config from "@/config";

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
const predefinedValues = usePredefinedValuesStore();
const userStore = useUserStore();
const appStore = useAppStore();
const authStore = useAuthStore();
const { genders, languages } = storeToRefs(predefinedValues);
const { loading } = storeToRefs(appStore);
const { user } = storeToRefs(authStore);

const showPassword = ref(false);

const schema = object().shape({
  gender: string().required(t("admin.users.validation.gender.required")),
  email: string().required(t("admin.users.validation.email.required")).email(t("admin.users.validation.email.invalid")),
  firstName: string().required(t("admin.users.validation.firstName.required")).max(100, t("admin.users.validation.firstName.maxLength")),
  lastName: string().required(t("admin.users.validation.lastName.required")).max(100, t("admin.users.validation.lastName.maxLength")),
  password: string()
    .required(t("admin.users.validation.password.required"))
    .min(8, t("admin.users.validation.password.minLength"))
    .matches(/^[A-Z]+$/, t("admin.users.validation.password.uppercase"))
    .matches(/^[a-z]+$/, t("admin.users.validation.password.lowercase"))
    .matches(/^[0-9]+$/, t("admin.users.validation.password.number"))
    .matches(/^[A-Za-z0-9!@#$%^&*()_+|~\-={}[\]:";<>?,./]+$/, t("admin.users.validation.password.special")),
  confirmPassword: string()
    .required(t("admin.users.validation.confirmPassword.required"))
    .oneOf([ref("password"), null], t("admin.users.validation.confirmPassword.match")),
  jobTitle: string().nullable().max(500, t("admin.users.validation.jobTitle.maxLength"))
});

const defaultValues = {
  tenantId: "",
  gender: "none",
  email: "",
  password: "",
  firstName: "",
  lastName: "",
  jobTitle: "",
  language: config.defaultLocale,
  permissions: []
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

const [gender, genderProps] = defineField("gender", vuetifyConfig);
const [email, emailProps] = defineField("email", vuetifyConfig);
const [password, passwordProps] = defineField("password", vuetifyConfig);
const [confirmPassword, confirmPasswordProps] = defineField("confirmPassword", vuetifyConfig);
const [firstName, firstNameProps] = defineField("firstName", vuetifyConfig);
const [lastName, lastNameProps] = defineField("lastName", vuetifyConfig);
const [jobTitle, jobTitleProps] = defineField("jobTitle", vuetifyConfig);
const [language, languageProps] = defineField("language", vuetifyConfig);
const [permissions, permissionsProps] = defineField("permissions", vuetifyConfig);

const onSubmit = handleSubmit(async (values) => {
  try {
    appStore.setPageLoader(true);
    const submitData = {
      ...values
    };

    if (props.isEdit) {
      await userStore.update(props.initialData.id, submitData);
    } else {
      await userStore.create(submitData);
    }

    router.push({ name: "list-users" });
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

onMounted(async () => {
  await predefinedValues.get();
});

const groupedPermissions = computed(() => {
  const modules = {};
  user.value.permissions.forEach((permission) => {
    const [module] = permission.split(".");
    if (!modules[module]) {
      modules[module] = [];
    }
    modules[module].push(permission);
  });
  return modules;
});
</script>

<template>
  <v-form @submit="onSubmit">
    <v-row>
      <v-col col="12" sm="8" md="9">
        <v-card>
          <v-toolbar color="secondary" dark>
            <v-toolbar-title :text="t('admin.users.helpers.information')" />
          </v-toolbar>
          <v-row class="pa-4 mt-2">
            <v-col cols="12" md="3">
              <v-select
                v-model="language"
                v-bind="languageProps"
                :items="languages || []"
                :label="$t('admin.users.fields.language')"
                :loading="userStore.loading"
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
                :loading="userStore.loading"
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
                :loading="userStore.loading"
                density="comfortable"
                variant="outlined"
              />
            </v-col>

            <v-col cols="12" md="6">
              <v-text-field
                v-model="lastName"
                v-bind="lastNameProps"
                :label="$t('admin.users.fields.lastName')"
                :loading="userStore.loading"
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

        <v-card class="mt-4">
          <v-toolbar color="secondary" dark>
            <v-toolbar-title :text="t('admin.users.helpers.permissions')" />
          </v-toolbar>

          <v-row class="pa-4 mt-2">
            <v-col cols="12">
              <div class="permissions-group">
                <div v-for="(modulePermissions, moduleName) in groupedPermissions" :key="moduleName" class="module-section mb-4">
                  <div class="text-h5 mb-2 text-capitalize">{{ moduleName }}</div>
                  <v-row>
                    <v-col v-for="permission in modulePermissions" :key="permission" cols="auto" xs="6" class="me-sm-auto">
                      <v-checkbox
                        v-model="permissions"
                        :value="permission"
                        :label="permission.split('.')[1]"
                        density="comfortable"
                        color="primary"
                        hide-details
                      />
                    </v-col>
                  </v-row>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-card>
      </v-col>

      <v-col col="12" sm="4" md="3">
        <v-card class="pa-4">
          <v-container :fluid="true">
            <v-row>
              <v-col>
                <v-btn block color="primary" type="submit" :loading="loading" :prepend-icon="isEdit ? '$pencil' : '$contentSave'">
                  {{ isEdit ? t("common.update") : t("common.save") }}
                </v-btn>
              </v-col>
            </v-row>
            <v-row>
              <v-col>
                <v-btn block color="error" :disabled="loading" @click="handleReset" prepend-icon="$refresh">
                  {{ t("common.reset") }}
                </v-btn>
              </v-col>
            </v-row>
          </v-container>
        </v-card>
      </v-col>
    </v-row>
  </v-form>
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

.permissions-group {
  .module-section {
    border-bottom: 1px solid rgba(0, 0, 0, 0.12);
    padding-bottom: 1rem;

    &:last-child {
      border-bottom: none;
    }
  }
}
</style>
