<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { toTypedSchema } from "@vee-validate/yup";
import { object, string } from "yup";
import { useForm } from "vee-validate";
import { User } from "@/models/user";

import { useAppStore } from "@/stores/app";
const appStore = useAppStore();
const { loading, genders, languages } = storeToRefs(appStore);

onMounted(async () => {
  await appStore.getPredefinedValues();
});

const { t } = useI18n();

const props = defineProps<{
  user: User;
  pageTitle: string;
  loading: boolean;
  routeName: RouteRecordNameGeneric;
}>();

const schema = toTypedSchema(
  object({
    firstName: string().required(t("admin.users.fields.firstName.required")),
    lastName: string().required(t("admin.users.fields.lastName.required")),
    email: string()
      .email(t("admin.users.fields.email.invalid"))
      .required(t("admin.users.fields.email.required")),
    birthday: string().nullable(),
    placeOfBirth: string().nullable(),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<User>({
  validationSchema: schema,
});

const readOnly: Ref<boolean> = ref(props.routeName === "user-view");

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
  (userName) => {
    if (userName) {
      resetForm({ values: userName });
    }
  },
  {
    immediate: true,
    deep: true,
  },
);

const [language] = defineField("language");
const [gender] = defineField("gender");
const [firstName, firstNameAttrs] = defineField("firstName");
const [lastName, lastNameAttrs] = defineField("lastName");
const [email, emailAttrs] = defineField("email");
const [phoneNumber] = defineField("phoneNumber");
const [birthday, birthdayAttrs] = defineField("birthday");
const [placeOfBirth, placeOfBirthAttrs] = defineField("placeOfBirth");

const emit = defineEmits<{
  (e: "submit", values: User): void;
}>();

const submit = handleSubmit((values: User) => {
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
          to: { name: 'user-list' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-form :disabled="readOnly">
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="gender"
              class="form-label"
              v-text="t('admin.users.fields.gender.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-select
              v-model="gender"
              hide-details
              item-text="value"
              item-title="text"
              variant="outlined"
              :disabled="loading || readOnly"
              :items="gendersOptions"
              :label="t('admin.personal.profile.fields.gender.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="firstName"
              class="form-label"
              v-text="t('admin.users.fields.firstName.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="firstName"
              v-bind="firstNameAttrs"
              variant="outlined"
              :disabled="loading || readOnly"
              :label="t('admin.users.fields.firstName.title')"
              :error-messages="errors.firstName"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="lastName"
              class="form-label"
              v-text="t('admin.users.fields.lastName.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="lastName"
              v-bind="lastNameAttrs"
              variant="outlined"
              :disabled="loading || readOnly"
              :label="t('admin.users.fields.lastName.title')"
              :error-messages="errors.lastName"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="email"
              v-text="t('admin.users.fields.email.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="email"
              v-bind="emailAttrs"
              variant="outlined"
              :disabled="loading || readOnly"
              :label="t('admin.users.fields.email.title')"
              :error-messages="errors.email"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="phoneNumber"
              v-text="t('admin.users.fields.phoneNumber.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="phoneNumber"
              variant="outlined"
              :disabled="loading || readOnly"
              :label="t('admin.users.fields.phoneNumber.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="language"
              class="form-label"
              v-text="t('admin.users.fields.language.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-select
              v-model="language"
              hide-details
              item-text="value"
              item-title="text"
              variant="outlined"
              :disabled="loading || readOnly"
              :items="languagesOptions"
              :label="t('admin.personal.profile.fields.language.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="birthday"
              v-text="t('admin.users.fields.birthday.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="birthday"
              v-bind="birthdayAttrs"
              type="date"
              variant="outlined"
              hide-details
              :disabled="loading || readOnly"
              :label="t('admin.users.fields.birthday.title')"
              :error-messages="errors.birthday"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="placeOfBirth"
              v-text="t('admin.users.fields.placeOfBirth.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="placeOfBirth"
              v-bind="placeOfBirthAttrs"
              variant="outlined"
              :disabled="loading || readOnly"
              :label="t('admin.users.fields.placeOfBirth.title')"
              :error-messages="errors.placeOfBirth"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <v-btn
              v-if="!readOnly"
              color="primary"
              variant="tonal"
              size="small"
              prepend-icon="bx-save"
              :loading="loading"
              @click="submit"
            >
              {{ t("shared.save") }}
            </v-btn>
            <v-btn
              v-else
              color="warning"
              size="small"
              prepend-icon="bx-lock-open-alt"
              :loading="loading"
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
