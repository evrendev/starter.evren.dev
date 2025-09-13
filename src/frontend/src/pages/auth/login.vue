<script setup lang="ts">
import { ref } from "vue";
import { LoginRequest, TwoFactorAuthRequest } from "@/requests/auth";
import { object, string, boolean } from "yup";
import { useForm } from "vee-validate";
import { toTypedSchema } from "@vee-validate/yup";
import { useAppStore } from "@/stores/app";
import { useAuthStore } from "@/stores/auth";
import { Result } from "@/primitives/result";
import { AccessTokenResponse } from "@/responses/auth";
import { Notify } from "@/stores/notification";
import Logo from "@/components/shared/Logo.vue";
import RecaptchaButton from "@/views/admin/authentication/RecaptchaButton.vue";
import authV1BottomShape from "@images/svg/auth-v1-bottom-shape.svg?url";
import authV1TopShape from "@images/svg/auth-v1-top-shape.svg?url";

const appStore = useAppStore();
const authStore = useAuthStore();
const router = useRouter();

const { loading } = storeToRefs(appStore);

const { t } = useI18n();

const isPasswordVisible = ref<boolean>(false);
const siteKey = ref<string>(import.meta.env.VITE_RECAPTCHA_SITE_KEY_V3 || "");
const showTwoFactorAuthModal = ref<boolean>(false);

const loginValidationSchema = toTypedSchema(
  object({
    email: string()
      .email(t("auth.login.email.invalid"))
      .required(t("auth.login.email.required"))
      .label(t("auth.login.email.label")),
    password: string()
      .required(t("auth.login.password.required"))
      .label(t("auth.login.password.label")),
    rememberMe: boolean().default(false),
  }),
);

const { defineField, handleSubmit, setFieldValue, errors } =
  useForm<LoginRequest>({
    validationSchema: loginValidationSchema,
  });

const [email, emailAttrs] = defineField("email");
const [password, passwordAttrs] = defineField("password");
const [rememberMe, rememberMeAttrs] = defineField("rememberMe");

const login = handleSubmit(async (values: LoginRequest) => {
  appStore.setLoading(true);

  try {
    const result: Result<AccessTokenResponse> = await authStore.login(values);

    if (result.succeeded && result.data?.twoFactorAuthRequired) {
      showTwoFactorAuthModal.value = true;
    } else if (result.succeeded) {
      Notify.success(t("auth.login.success"));
      router.push({ name: "dashboard" });
    } else {
      appStore.setLoading(false);
      Notify.error(result.errors?.message || t("auth.login.error"));
    }
  } catch (error) {
    Notify.error((error as Error).message || t("auth.login.failed"));
  } finally {
    appStore.setLoading(false);
  }
});

const twoFactorAuthenticationValidationSchema = toTypedSchema(
  object().shape({
    code: string()
      .required(t("auth.twoFactorAuth.required"))
      .label(t("auth.twoFactorAuth.label"))
      .matches(/^[0-9]{6}$/, t("auth.twoFactorAuth.code.invalid")),
  }),
);

const {
  defineField: define2FAField,
  handleSubmit: handle2FASubmit,
  errors: twoFAErrors,
} = useForm<TwoFactorAuthRequest>({
  validationSchema: twoFactorAuthenticationValidationSchema,
});

const [code, codeAttrs] = define2FAField("code");

const checkTwoFactorAuth = handle2FASubmit(
  async (values: TwoFactorAuthRequest) => {
    appStore.setLoading(true);
    values.email = email.value;

    try {
      const result: Result<AccessTokenResponse> =
        await authStore.verifyTwoFactorAuth(values);

      if (result.succeeded) {
        Notify.success(t("auth.login.success"));
        showTwoFactorAuthModal.value = false;
        router.push({ name: "dashboard" });
      } else {
        Notify.error(
          result.errors?.message || t("auth.twoFactorAuth.verificationFailed"),
        );
      }
    } catch (error) {
      Notify.error(
        (error as Error).message || t("auth.twoFactorAuth.verificationError"),
      );
    } finally {
      appStore.setLoading(false);
    }
  },
);

const handleRecaptchaSuccess = (token: string) => {
  setFieldValue("response", token);
  login();
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

        <v-card-text>
          <h4 class="text-h4 mb-1 text-center">
            {{ t("auth.login.welcome") }}
          </h4>
          <p class="mb-0 text-center">
            {{ t("auth.login.subtitle") }}
          </p>
        </v-card-text>

        <v-card-text>
          <v-form>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="email"
                  v-bind="emailAttrs"
                  type="email"
                  placeholder="@"
                  :label="t('auth.login.email.label')"
                  :error-messages="errors.email"
                  :disabled="loading"
                  autofocus
                />
              </v-col>

              <v-col cols="12">
                <v-text-field
                  v-model="password"
                  v-bind="passwordAttrs"
                  placeholder="············"
                  autocomplete="password"
                  :label="t('auth.login.password.label')"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="isPasswordVisible ? 'bx-hide' : 'bx-show'"
                  :error-messages="errors.password"
                  :disabled="loading"
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
              </v-col>

              <v-col
                cols="12"
                class="d-flex align-center justify-space-between flex-wrap"
              >
                <v-checkbox
                  v-model="rememberMe"
                  v-bind="rememberMeAttrs"
                  :disabled="loading"
                  :label="t('auth.login.rememberMe')"
                />
                <router-link
                  class="text-primary"
                  :to="{ name: 'forgot-password' }"
                >
                  {{ t("auth.login.forgotPassword") }}
                </router-link>
              </v-col>

              <v-col cols="12">
                <recaptcha-button
                  action="submit"
                  button-icon="bx bx-log-in"
                  :block="true"
                  :button-text="t('auth.login.submit')"
                  :loading="loading"
                  :site-key="siteKey"
                  @recaptcha-success="handleRecaptchaSuccess"
                  @recaptcha-error="handleRecaptchaError"
                />
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
      </v-card>
    </div>

    <modal-window
      :show-modal="showTwoFactorAuthModal"
      :title="t('auth.twoFactorAuth.title')"
    >
      <template #content>
        <v-row>
          <v-col col="12" class="d-flex align-center justify-center">
            <v-otp-input
              v-model="code"
              v-bind="codeAttrs"
              :error-messages="twoFAErrors.code"
              :disabled="loading"
              autofocus
            />
          </v-col>
        </v-row>
      </template>

      <template #action-buttons>
        <v-btn
          color="primary"
          variant="elevated"
          size="small"
          :loading="loading"
          @click="checkTwoFactorAuth"
        >
          {{ t("shared.submit") }}
        </v-btn>

        <v-btn
          text
          :disabled="loading"
          @click="showTwoFactorAuthModal = false"
          size="small"
        >
          {{ t("shared.cancel") }}
        </v-btn>
      </template>
    </modal-window>
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/admin/template/pages/page-auth";
</style>
