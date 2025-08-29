<script setup lang="ts">
import { ref } from "vue";
import { LoginRequest } from "@/requests/auth";
import { object, string, boolean } from "yup";
import { useForm } from "vee-validate";
import { toTypedSchema } from "@vee-validate/yup";
import { useAppStore } from "@/stores/app";
import { useAuthStore } from "@/stores/auth";
import { Result } from "@/primitives/result";
import { AccessTokenResponse } from "@/responses/auth";
import { Notify } from "@/stores/notification";
import Logo from "@/components/admin/Logo.vue";
import RecaptchaButton from "@/views/pages/authentication/RecaptchaButton.vue";
import authV1BottomShape from "@images/svg/auth-v1-bottom-shape.svg?url";
import authV1TopShape from "@images/svg/auth-v1-top-shape.svg?url";

const appStore = useAppStore();
const authStore = useAuthStore();
const router = useRouter();

const { loading } = storeToRefs(appStore);

const { t } = useI18n();

const initialValues: LoginRequest = {
  email: "",
  password: "",
  response: "",
  rememberMe: false,
};

const schema = toTypedSchema(
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
    validationSchema: schema,
    initialValues: initialValues,
  });

const [email, emailAttrs] = defineField("email");
const [password, passwordAttrs] = defineField("password");
const [rememberMe, rememberMeAttrs] = defineField("rememberMe");

const login = handleSubmit(async (values: LoginRequest) => {
  appStore.setLoading(true);
  const result: Result<AccessTokenResponse> = await authStore.login(values);

  if (result.succeeded) {
    Notify.success(t("auth.login.success"));
    router.push({ name: "dashboard" });
    appStore.setLoading(false);
  } else {
    appStore.setLoading(false);
    Notify.error(result.errors?.message || t("auth.login.error"));
  }
});

const handleRecaptchaSuccess = (token: string) => {
  setFieldValue("response", token);
  login();
};

const handleRecaptchaError = (error: Error) => {
  console.error("reCAPTCHA hatası:", error);
};

const isPasswordVisible = ref<boolean>(false);
const siteKey = ref<string>(import.meta.env.VITE_RECAPTCHA_SITE_KEY_V3 || "");
</script>

<template>
  <div class="auth-wrapper d-flex align-center justify-center pa-4">
    <div class="position-relative my-sm-16">
      <VImg
        :src="authV1TopShape"
        class="text-primary auth-v1-top-shape d-none d-sm-block"
      />
      <VImg
        :src="authV1BottomShape"
        class="text-primary auth-v1-bottom-shape d-none d-sm-block"
      />

      <VCard
        class="auth-card"
        max-width="460"
        :class="$vuetify.display.smAndUp ? 'pa-6' : 'pa-0'"
      >
        <VCardItem class="justify-center">
          <Logo />
        </VCardItem>

        <VCardText>
          <h4 class="text-h4 mb-1 text-center">
            {{ t("auth.login.welcome") }}
          </h4>
          <p class="mb-0 text-center">
            {{ t("auth.login.subtitle") }}
          </p>
        </VCardText>

        <VCardText>
          <VForm>
            <VRow>
              <VCol cols="12">
                <VTextField
                  v-model="email"
                  v-bind="emailAttrs"
                  type="email"
                  placeholder="@"
                  :label="t('auth.login.email.label')"
                  :error-messages="errors.email"
                  :disabled="loading"
                  autofocus
                />
              </VCol>

              <VCol cols="12">
                <VTextField
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
              </VCol>

              <VCol
                cols="12"
                class="d-flex align-center justify-space-between flex-wrap"
              >
                <VCheckbox
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
              </VCol>

              <VCol cols="12">
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
              </VCol>

              <VCol cols="12">
                <VAlert
                  v-if="errors.response"
                  type="error"
                  class="mt-4"
                  density="compact"
                >
                  {{ errors.response }}
                </VAlert>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCard>
    </div>
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/admin/template/pages/page-auth";
</style>
