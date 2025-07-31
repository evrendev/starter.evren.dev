<script setup lang="ts">
import { ref as vueRef, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { ResetPasswordRequest } from "@/requests/user";
import { object, string, ref } from "yup";
import { useForm } from "vee-validate";
import { toTypedSchema } from "@vee-validate/yup";
import { useAppStore } from "@/stores/app";
import { useUserStore } from "@/stores/user";
import { Notify } from "@/stores/notification";
import logo from "@images/logo.svg?raw";
import authV1BottomShape from "@images/svg/auth-v1-bottom-shape.svg?url";
import authV1TopShape from "@images/svg/auth-v1-top-shape.svg?url";

const appStore = useAppStore();
const userStore = useUserStore();
const router = useRouter();
const route = useRoute();

const { loading } = storeToRefs(appStore);

const { t } = useI18n();

const initialValues: ResetPasswordRequest = {
  email: String(route.query.email || ""),
  password: "",
  token: String(route.query.token || ""),
};

const schema = toTypedSchema(
  object({
    password: string()
      .required(t("auth.reset-password.password.required"))
      .min(8, t("auth.reset-password.password.min-length"))
      .matches(
        /^[A-Za-z0-9!@#$%^&*()_+|~\-={}[\]:";<>?,./]+$/,
        t("auth.reset-password.password.special"),
      )
      .label(t("auth.reset-password.password.label")),
    confirmPassword: string()
      .oneOf(
        [ref("password")],
        t("auth.reset-password.confirm-password.required"),
      )
      .required(t("auth.reset-password.confirm-password.required"))
      .label(t("auth.reset-password.confirm-password.label")),
    email: string().required(t("auth.reset-password.email.required")),
    token: string().required(t("auth.reset-password.token.required")),
  }),
);

const { defineField, handleSubmit, setFieldValue, errors } =
  useForm<ResetPasswordRequest>({
    validationSchema: schema,
    initialValues: initialValues,
  });

const [password, passwordAttrs] = defineField("password");
const [confirmPassword] = defineField("confirmPassword");

const submit = handleSubmit(async (values) => {
  appStore.setLoading(true);
  const result: string = await userStore.resetPassword(values);

  if (result) {
    appStore.setLoading(false);
    Notify.success(t("auth.reset-password.success"));
    router.replace({ name: "login" });
  } else {
    appStore.setLoading(false);
    Notify.error(t("auth.reset-password.error"));
  }
});

onMounted(() => {
  setFieldValue("email", String(route.query.email || ""));
  setFieldValue("token", String(route.query.token || ""));
});

const isPasswordVisible = vueRef<boolean>(false);
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
          <RouterLink to="/" class="app-logo">
            <div class="d-flex" v-html="logo" />
            <h1 class="app-logo-title" v-text="t('app.title')" />
          </RouterLink>
        </VCardItem>

        <VCardText>
          <h4 class="text-h4 mb-1 text-center">
            {{ t("auth.reset-password.welcome") }}
          </h4>
          <p class="mb-0 text-center">
            {{ t("auth.reset-password.subtitle") }}
          </p>
        </VCardText>

        <VCardText>
          <VForm>
            <VRow>
              <VCol cols="12">
                <VTextField
                  v-model="password"
                  v-bind="passwordAttrs"
                  placeholder="············"
                  autocomplete="password"
                  :label="t('auth.reset-password.password.label')"
                  :error-messages="errors.password"
                  :disabled="loading"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="isPasswordVisible ? 'bx-hide' : 'bx-show'"
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
              </VCol>
              <VCol cols="12">
                <VTextField
                  v-model="confirmPassword"
                  type="password"
                  :label="t('auth.reset-password.confirm-password.label')"
                  :error-messages="errors.confirmPassword"
                  :disabled="loading"
                />
              </VCol>
              <VCol cols="12">
                <v-btn
                  @click="submit"
                  prepend-icon="bx bx-log-in"
                  color="primary"
                  :block="true"
                  :loading="loading"
                  :text="t('auth.reset-password.submit')"
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
