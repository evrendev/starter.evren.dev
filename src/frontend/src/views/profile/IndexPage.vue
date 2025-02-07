<script setup>
import { shallowRef, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { storeToRefs } from "pinia";
import { useForm } from "vee-validate";
import { object, string } from "yup";
import { useAuthStore, usePredefinedValuesStore, useAppStore } from "@/stores";
import { Breadcrumb } from "@/components/forms";
import TwoFactorAuthDialog from "./components/TwoFactorAuthDialog.vue";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.profile.title"),
    disabled: true,
    href: "#"
  }
]);

const authStore = useAuthStore();
const predefinedValues = usePredefinedValuesStore();
const appStore = useAppStore();
const { genders, languages } = storeToRefs(predefinedValues);
const { user } = storeToRefs(authStore);
const disabled = ref(true);

const schema = object().shape({
  gender: string().required(t("admin.profile.validation.gender.required")),
  firstName: string()
    .required(t("admin.profile.validation.firstName.required"))
    .max(200, t("admin.profile.validation.firstName.maxLength")),
  lastName: string().required(t("admin.profile.validation.lastName.required")).max(200, t("admin.profile.validation.lastName.maxLength")),
  jobTitle: string().nullable().max(500, t("admin.profile.validation.jobTitle.maxLength")),
  language: string().required(t("admin.profile.validation.language.required"))
});

const { defineField, handleSubmit, resetForm } = useForm({
  validationSchema: schema,
  initialValues: user
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const [gender, genderProps] = defineField("gender", vuetifyConfig);
const [firstName, firstNameProps] = defineField("firstName", vuetifyConfig);
const [lastName, lastNameProps] = defineField("lastName", vuetifyConfig);
const [jobTitle, jobTitleProps] = defineField("jobTitle", vuetifyConfig);
const [language, languageProps] = defineField("language", vuetifyConfig);

const onSubmit = handleSubmit(async (values) => {
  try {
    appStore.setPageLoader(true);
    const submitData = {
      ...values
    };

    await authStore.update(user.id, submitData);
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoader(false);
  }
});

const handleReset = () => {
  disabled.value = true;
  resetForm();
};

const showTwoFactorDialog = ref(false);

const handleTwoFactorEnabled = async () => {
  try {
    appStore.setPageLoader(true);
    await authStore.getCurrentUser();
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoader(false);
  }
};

const handleTwoFactorDisabled = async () => {
  try {
    appStore.setPageLoader(true);
    await authStore.getCurrentUser();
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoader(false);
  }
};

onMounted(async () => {
  await predefinedValues.get();
});
</script>

<template>
  <breadcrumb :title="t('admin.profile.title')" :breadcrumbs="breadcrumbs" />

  <v-row>
    <v-col cols="12" class="mx-auto">
      <v-card class="pa-6">
        <v-form @submit="onSubmit">
          <v-row class="mt-2">
            <v-col cols="12" class="text-center">
              <v-avatar size="120" color="primary">
                <span v-if="user?.image" class="text-h1 text-white fw-bold">
                  {{ user?.initial }}
                </span>
                <v-img v-else :src="user.image" />
              </v-avatar>
            </v-col>

            <v-col cols="12" class="text-center text-caption font-weight-thin">
              {{ user?.email }}
            </v-col>

            <v-col cols="12" md="2">
              <v-select
                v-model="gender"
                v-bind="genderProps"
                :items="genders || []"
                :label="$t('admin.profile.gender')"
                :loading="authStore.loading"
                :disabled="disabled"
                density="comfortable"
                variant="outlined"
                item-title="name"
                item-value="code"
              />
            </v-col>

            <v-col cols="12" md="5">
              <v-text-field
                v-model="firstName"
                v-bind="firstNameProps"
                :label="$t('admin.profile.firstName')"
                :loading="authStore.loading"
                :disabled="disabled"
                density="comfortable"
                variant="outlined"
              />
            </v-col>

            <v-col cols="12" md="5">
              <v-text-field
                v-model="lastName"
                v-bind="lastNameProps"
                :label="$t('admin.profile.lastName')"
                :loading="authStore.loading"
                :disabled="disabled"
                density="comfortable"
                variant="outlined"
              />
            </v-col>

            <v-col cols="12" md="2">
              <v-select
                v-model="language"
                v-bind="languageProps"
                :items="languages || []"
                :label="$t('admin.profile.language')"
                :loading="authStore.loading"
                :disabled="disabled"
                item-title="name"
                item-value="code"
                density="comfortable"
                variant="outlined"
              />
            </v-col>

            <v-col cols="12" md="10">
              <v-text-field
                v-model="jobTitle"
                v-bind="jobTitleProps"
                :label="$t('admin.profile.jobTitle')"
                :loading="authStore.loading"
                :disabled="disabled"
                density="comfortable"
                variant="outlined"
              />
            </v-col>
          </v-row>

          <v-divider class="my-4" />

          <v-row>
            <v-col cols="12">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <h3 class="text-h6 mb-2">{{ t("admin.profile.twoFactorAuth.title") }}</h3>
                  <p class="text-body-2">{{ t("admin.profile.twoFactorAuth.description") }}</p>
                </div>
                <div>
                  <v-btn v-if="!user.twoFactorEnabled" color="primary" prepend-icon="$shieldAccount" @click="showTwoFactorDialog = true">
                    {{ t("admin.profile.twoFactorAuth.enable") }}
                  </v-btn>
                  <v-btn v-else color="error" prepend-icon="$shieldOff" @click="handleTwoFactorDisabled">
                    {{ t("admin.profile.twoFactorAuth.disable") }}
                  </v-btn>
                </div>
              </div>
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12" class="d-flex justify-end gap-2">
              <v-btn color="warning" v-show="disabled" prepend-icon="$pencil" @click="disabled = false">
                {{ t("common.edit") }}
              </v-btn>

              <v-btn color="error" :disabled="authStore.loading" v-show="!disabled" @click="handleReset" prepend-icon="$refresh">
                {{ t("common.reset") }}
              </v-btn>
              <v-btn color="primary" type="submit" :loading="authStore.loading" prepend-icon="$contentSave" class="ml-2" v-show="!disabled">
                {{ t("common.save") }}
              </v-btn>
            </v-col>
          </v-row>
        </v-form>

        <two-factor-auth-dialog v-model="showTwoFactorDialog" @enabled="handleTwoFactorEnabled" @disabled="handleTwoFactorDisabled" />
      </v-card>
    </v-col>
  </v-row>
</template>
