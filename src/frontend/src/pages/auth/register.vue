<script setup lang="ts">
import { ref } from "vue";
import { SelfRegisterRequest } from "@/types/requests/user";
import { object, string, ref as yupRef } from "yup";
import { useForm } from "vee-validate";
import { toTypedSchema } from "@vee-validate/yup";
import { useAppStore } from "@/stores/app";
import { useUserStore } from "@/stores/user";
import { Notify } from "@/stores/notification";
import Logo from "@/components/shared/Logo.vue";
import RecaptchaButton from "@/views/admin/authentication/RecaptchaButton.vue";
import authV1BottomShape from "@images/svg/auth-v1-bottom-shape.svg?url";
import authV1TopShape from "@images/svg/auth-v1-top-shape.svg?url";

const router = useRouter();
const appStore = useAppStore();
const userStore = useUserStore();
const { loading } = storeToRefs(appStore);

const { t } = useI18n();

const isPasswordVisible = ref<boolean>(false);
const isConfirmPasswordVisible = ref<boolean>(false);
const registered = ref<boolean>(false);
const siteKey = ref<string>(import.meta.env.VITE_RECAPTCHA_SITE_KEY_V3 || "");

// Gender is left unset (None) and language defaults to the site's default
// language — neither is exposed on the registration form.
const DEFAULT_LANGUAGE = import.meta.env.VITE_APP_DEFAULT_LANGUAGE as string;
const LANGUAGE_CODES: Record<string, number> = { en: 1, tr: 2, de: 3 };

const registerValidationSchema = toTypedSchema(
  object({
    firstName: string()
      .required(t("auth.register.fields.firstName.required"))
      .label(t("auth.register.fields.firstName.label")),
    lastName: string()
      .required(t("auth.register.fields.lastName.required"))
      .label(t("auth.register.fields.lastName.label")),
    email: string()
      .email(t("auth.register.fields.email.invalid"))
      .required(t("auth.register.fields.email.required"))
      .label(t("auth.register.fields.email.label")),
    password: string()
      .required(t("auth.register.fields.password.required"))
      .min(8, t("auth.register.fields.password.min-length"))
      .matches(
        /^[A-Za-z0-9!@#$%^&*()_+|~\-={}[\]:";<>?,./]+$/,
        t("auth.register.fields.password.special"),
      )
      .label(t("auth.register.fields.password.label")),
    confirmPassword: string()
      .oneOf(
        [yupRef("password")],
        t("auth.register.fields.confirmPassword.invalid"),
      )
      .required(t("auth.register.fields.confirmPassword.required"))
      .label(t("auth.register.fields.confirmPassword.label")),
  }),
);

const { defineField, handleSubmit, setFieldValue, errors } =
  useForm<SelfRegisterRequest>({
    validationSchema: registerValidationSchema,
  });

const [firstName, firstNameAttrs] = defineField("firstName");
const [lastName, lastNameAttrs] = defineField("lastName");
const [email, emailAttrs] = defineField("email");
const [password, passwordAttrs] = defineField("password");
const [confirmPassword, confirmPasswordAttrs] = defineField("confirmPassword");

const register = handleSubmit(async (values: SelfRegisterRequest) => {
  appStore.setLoading(true);
  try {
    values.gender = 0;
    values.language = LANGUAGE_CODES[DEFAULT_LANGUAGE] || 1;

    await userStore.selfRegister(values);
    registered.value = true;
    Notify.success(t("auth.register.successMessage"));
  } catch (error) {
    Notify.error((error as Error).message || t("auth.register.error"));
  } finally {
    appStore.setLoading(false);
  }
});

const handleRecaptchaSuccess = (token: string) => {
  setFieldValue("response", token);
  register();
};

const handleRecaptchaError = (error: Error) => {
  Notify.error(error.message || t("auth.login.recaptchaError"));
};
</script>

<template>
  <div class="auth-wrapper d-flex align-center justify-center pa-4">
    <div class="position-relative my-sm-16">
      <v-img
        :src="authV1TopShape"
        class="text-primary auth-v1-top-shape d-none d-sm-block"
      />
      <v-img
        :src="authV1BottomShape"
        class="text-primary auth-v1-bottom-shape d-none d-sm-block"
      />

      <v-card
        class="auth-card"
        max-width="460"
        :class="$vuetify.display.smAndUp ? 'pa-6' : 'pa-0'"
      >
        <v-card-item class="justify-center">
          <logo />
        </v-card-item>

        <v-card-text v-if="registered">
          <h4 class="text-h4 mb-1 text-center">
            {{ t("auth.register.welcome") }}
          </h4>
          <v-alert type="success" class="mt-4" density="compact">
            {{ t("auth.register.successMessage") }}
          </v-alert>
          <v-btn
            class="mt-4"
            color="primary"
            variant="flat"
            :block="true"
            :to="{ name: 'login' }"
          >
            {{ t("auth.forgot-password.back-to-login") }}
          </v-btn>
        </v-card-text>

        <template v-else>
          <v-card-text>
            <h4 class="text-h4 mb-1 text-center">
              {{ t("auth.register.welcome") }}
            </h4>
            <p class="mb-0 text-center">
              {{ t("auth.register.subtitle") }}
            </p>
          </v-card-text>

          <v-card-text>
            <v-form>
              <v-row>
                <v-col cols="12">
                  <v-text-field
                    v-model="firstName"
                    v-bind="firstNameAttrs"
                    :label="t('auth.register.fields.firstName.label')"
                    :placeholder="t('auth.register.fields.firstName.placeholder')"
                    :error-messages="errors.firstName"
                    :disabled="loading"
                    autofocus
                  />
                </v-col>

                <v-col cols="12">
                  <v-text-field
                    v-model="lastName"
                    v-bind="lastNameAttrs"
                    :label="t('auth.register.fields.lastName.label')"
                    :placeholder="t('auth.register.fields.lastName.placeholder')"
                    :error-messages="errors.lastName"
                    :disabled="loading"
                  />
                </v-col>

                <v-col cols="12">
                  <v-text-field
                    v-model="email"
                    v-bind="emailAttrs"
                    type="email"
                    :label="t('auth.register.fields.email.label')"
                    :placeholder="t('auth.register.fields.email.placeholder')"
                    :error-messages="errors.email"
                    :disabled="loading"
                  />
                </v-col>

                <v-col cols="12">
                  <v-text-field
                    v-model="password"
                    v-bind="passwordAttrs"
                    autocomplete="new-password"
                    :label="t('auth.register.fields.password.label')"
                    :placeholder="t('auth.register.fields.password.placeholder')"
                    :type="isPasswordVisible ? 'text' : 'password'"
                    :append-inner-icon="isPasswordVisible ? 'bx-hide' : 'bx-show'"
                    :error-messages="errors.password"
                    :disabled="loading"
                    @click:append-inner="isPasswordVisible = !isPasswordVisible"
                  />
                </v-col>

                <v-col cols="12">
                  <v-text-field
                    v-model="confirmPassword"
                    v-bind="confirmPasswordAttrs"
                    autocomplete="new-password"
                    :label="t('auth.register.fields.confirmPassword.label')"
                    :placeholder="t('auth.register.fields.confirmPassword.placeholder')"
                    :type="isConfirmPasswordVisible ? 'text' : 'password'"
                    :append-inner-icon="isConfirmPasswordVisible ? 'bx-hide' : 'bx-show'"
                    :error-messages="errors.confirmPassword"
                    :disabled="loading"
                    @click:append-inner="isConfirmPasswordVisible = !isConfirmPasswordVisible"
                  />
                </v-col>

                <v-col cols="12">
                  <recaptcha-button
                    action="submit"
                    button-icon="bx bx-user-plus"
                    :block="true"
                    :button-text="t('auth.register.submit')"
                    :loading="loading"
                    :site-key="siteKey"
                    @recaptcha-success="handleRecaptchaSuccess"
                    @recaptcha-error="handleRecaptchaError"
                  />
                </v-col>

                <v-col
                  cols="12"
                  class="d-flex align-center justify-center flex-wrap"
                >
                  <span class="me-1">{{ t("auth.register.haveAccount") }}</span>
                  <router-link class="text-primary" :to="{ name: 'login' }">
                    {{ t("auth.register.loginLink") }}
                  </router-link>
                </v-col>

                <v-col cols="12">
                  <v-alert
                    v-if="errors.response"
                    type="error"
                    class="mt-4"
                    density="compact"
                  >
                    {{ errors.response }}
                  </v-alert>
                </v-col>
              </v-row>
            </v-form>
          </v-card-text>
        </template>
      </v-card>
    </div>
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/admin/template/pages/page-auth";
</style>
