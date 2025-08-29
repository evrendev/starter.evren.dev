<script setup lang="ts">
import { ref } from "vue";
import { ForgotPasswordRequest } from "@/requests/user";
import { object, string } from "yup";
import { useForm } from "vee-validate";
import { toTypedSchema } from "@vee-validate/yup";
import { useAppStore } from "@/stores/app";
import { useUserStore } from "@/stores/user";
import { Notify } from "@/stores/notification";
import RecaptchaButton from "@/views/pages/authentication/RecaptchaButton.vue";
import Logo from "components/admin/Logo.vue";
import authV1BottomShape from "@images/svg/auth-v1-bottom-shape.svg?url";
import authV1TopShape from "@images/svg/auth-v1-top-shape.svg?url";

const appStore = useAppStore();
const userStore = useUserStore();
const router = useRouter();

const { loading } = storeToRefs(appStore);

const { t } = useI18n();

const initialValues: ForgotPasswordRequest = {
  email: "",
};

const schema = toTypedSchema(
  object({
    email: string()
      .email(t("auth.login.email.invalid"))
      .required(t("auth.login.email.required"))
      .label(t("auth.login.email.label")),
  }),
);

const { defineField, handleSubmit, setFieldValue, errors } =
  useForm<ForgotPasswordRequest>({
    validationSchema: schema,
    initialValues: initialValues,
  });

const [email, emailAttrs] = defineField("email");

const submit = handleSubmit(async (request) => {
  appStore.setLoading(true);
  const result: string = await userStore.forgotPassword(request.email);

  if (result) {
    appStore.setLoading(false);
    Notify.success(result);
    router.replace({ name: "login" });
  } else {
    appStore.setLoading(false);
    Notify.error(t("auth.forgotPassword.error"));
  }
});

const handleRecaptchaSuccess = (token: string) => {
  setFieldValue("response", token);
  submit();
};

const handleRecaptchaError = (error: Error) => {
  console.error("reCAPTCHA hatası:", error);
};

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
            {{ t("auth.forgot-password.welcome") }}
          </h4>
          <p class="mb-0 text-center">
            {{ t("auth.forgot-password.subtitle") }}
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
                  :placeholder="t('auth.forgot-password.email.placeholder')"
                  :label="t('auth.forgot-password.email.label')"
                  :error-messages="errors.email"
                  :disabled="loading"
                  autofocus
                />
              </VCol>
              <VCol cols="12">
                <recaptcha-button
                  action="submit"
                  button-icon="bx bx-log-in"
                  :block="true"
                  :button-text="t('auth.forgot-password.submit')"
                  :loading="loading"
                  :site-key="siteKey"
                  @recaptcha-success="handleRecaptchaSuccess"
                  @recaptcha-error="handleRecaptchaError"
                />
              </VCol>
              <VCol cols="12">
                <router-link
                  class="text-center d-block mt-4"
                  v-text="t('auth.forgot-password.back-to-login')"
                  :to="{ name: 'login' }"
                />
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCard>
    </div>
  </div>
</template>

<style lang="scss">
@use "@core/scss/template/pages/page-auth";
</style>
