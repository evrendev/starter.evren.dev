<script lang="ts" setup>
import { BasicUser } from "@/models/user";

import { toTypedSchema } from "@vee-validate/yup";
import { date, number, object, string } from "yup";
import { useForm } from "vee-validate";

import { useAppStore } from "@/stores/app";
const appStore = useAppStore();
const { genders, languages } = storeToRefs(appStore);

onMounted(async () => {
  await appStore.getPredefinedValues();
});

const { t } = useI18n();

const props = defineProps<{
  user: BasicUser | null;
  loading: boolean;
}>();

const schema = toTypedSchema(
  object({
    language: number()
      .required(t("admin.personel.profile.fields.language.required"))
      .oneOf(
        languages.value.map((language) => language.value),
        t("admin.personel.profile.fields.language.invalid"),
      ),
    gender: number()
      .required(t("admin.personel.profile.fields.gender.required"))
      .oneOf(
        genders.value.map((gender) => Number(gender.value)),
        t("admin.personel.profile.fields.gender.invalid"),
      ),
    birthday: date().nullable(),
    firstName: string().required(
      t("admin.personel.profile.fields.firstName.required"),
    ),
    lastName: string().required(
      t("admin.personel.profile.fields.lastName.required"),
    ),
    email: string()
      .email()
      .required(t("admin.personel.profile.fields.email.required")),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<BasicUser>({
  validationSchema: schema,
});

const gendersOptions = computed(() => {
  return genders.value.map((gender) => ({
    value: gender.value,
    text: t(`shared.options.genders.${gender.name}`),
  }));
});

const languagesOptions = computed(() => {
  return languages.value.map((language) => ({
    value: language.value,
    text: t(`shared.options.languages.${language.name}`),
  }));
});

watch(
  () => props.user,
  (user) => {
    if (user) {
      resetForm({ values: user });
    }
  },
  {
    immediate: true,
    deep: true,
  },
);

const [language, languageAttrs] = defineField("language");
const [gender, genderAttrs] = defineField("gender");
const [firstName, firstNameAttrs] = defineField("firstName");
const [lastName, lastNameAttrs] = defineField("lastName");
const [email, emailAttrs] = defineField("email");
const [phoneNumber, phoneNumberAttrs] = defineField("phoneNumber");
const [birthday, birthdayAttrs] = defineField("birthday");
const [placeOfBirth, placeOfBirthAttrs] = defineField("placeOfBirth");

const disabled: Ref<boolean> = ref(true);
const emit = defineEmits<{
  (e: "submit", values: BasicUser): void;
}>();

const submit = handleSubmit((values) => {
  emit("submit", values);
});

const reset = () => {
  resetForm();
  disabled.value = true;
};
</script>

<template>
  <v-row>
    <v-col cols="12">
      <v-card>
        <v-card-text class="d-flex align-center justify-center">
          <v-avatar rounded="lg" size="96" color="primary">
            <h1 class="text-white">{{ user?.initial }}</h1>
          </v-avatar>
        </v-card-text>

        <v-divider />

        <v-card-text>
          <v-form class="mt-6">
            <v-row>
              <v-col md="6" cols="12">
                <v-select
                  v-model="gender"
                  v-bind="genderAttrs"
                  hide-details
                  item-text="value"
                  item-title="text"
                  variant="outlined"
                  :disabled="disabled"
                  :items="gendersOptions"
                  :label="t('admin.personel.profile.fields.gender.title')"
                  :error-messages="errors.gender"
                />
              </v-col>
              <v-col md="6" cols="12">
                <v-select
                  v-model="language"
                  v-bind="languageAttrs"
                  hide-details
                  item-text="value"
                  item-title="text"
                  variant="outlined"
                  :disabled="disabled"
                  :items="languagesOptions"
                  :label="t('admin.personel.profile.fields.language.title')"
                  :error-messages="errors.language"
                />
              </v-col>

              <v-col md="6" cols="12">
                <v-text-field
                  v-model="firstName"
                  v-bind="firstNameAttrs"
                  :disabled="disabled"
                  :placeholder="user?.firstName"
                  :label="t('admin.personel.profile.fields.firstName.title')"
                  :error-messages="errors.firstName"
                />
              </v-col>

              <v-col md="6" cols="12">
                <v-text-field
                  v-bind="lastNameAttrs"
                  v-model="lastName"
                  :disabled="disabled"
                  :placeholder="user?.lastName"
                  :label="t('admin.personel.profile.fields.lastName.title')"
                  :error-messages="errors.lastName"
                />
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field
                  type="email"
                  v-bind="emailAttrs"
                  v-model="email"
                  :disabled="disabled"
                  :label="t('admin.personel.profile.fields.email.title')"
                  :placeholder="user?.email"
                  :error-messages="errors.email"
                />
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field
                  type="text"
                  v-bind="phoneNumberAttrs"
                  v-model="phoneNumber"
                  :disabled="disabled"
                  :label="t('admin.personel.profile.fields.phoneNumber.title')"
                  :placeholder="user?.phoneNumber"
                  :error-messages="errors.phoneNumber"
                />
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field
                  type="date"
                  v-bind="birthdayAttrs"
                  v-model="birthday"
                  :disabled="disabled"
                  :label="t('admin.personel.profile.fields.birthday.title')"
                  :error-messages="errors.birthday"
                />
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field
                  type="text"
                  v-bind="placeOfBirthAttrs"
                  v-model="placeOfBirth"
                  :disabled="disabled"
                  :label="t('admin.personel.profile.fields.placeOfBirth.title')"
                  :error-messages="errors.placeOfBirth"
                  :placeholder="user?.placeOfBirth"
                />
              </v-col>

              <v-col cols="12" class="d-flex flex-wrap gap-4">
                <v-btn
                  v-if="!disabled"
                  color="success"
                  size="small"
                  prepend-icon="bx-save"
                  @click="submit"
                >
                  {{ t("shared.save") }}
                </v-btn>

                <v-btn
                  v-if="disabled"
                  color="warning"
                  size="small"
                  prepend-icon="bx-lock-open-alt"
                  @click="disabled = false"
                >
                  {{ t("shared.enableEdit") }}
                </v-btn>

                <v-btn
                  v-if="!disabled"
                  size="small"
                  color="secondary"
                  variant="tonal"
                  prepend-icon="bx-reset"
                  @click="reset"
                >
                  Reset
                </v-btn>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>
