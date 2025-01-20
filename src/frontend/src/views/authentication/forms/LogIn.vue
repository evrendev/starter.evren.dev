<script setup>
import Logo from "@/layouts/full/logo/LogoDark.vue";
import { ref } from "vue";
import { useLocale } from "vuetify";
import { useForm } from "vee-validate";
import { object, string, boolean } from "yup";
import { ChallengeV3, useRecaptchaProvider } from "vue-recaptcha";
import { createToaster } from "@meforma/vue-toaster";
import { until } from "@vueuse/core";
import { useAuthStore } from "@/stores/auth";
import { useAppStore } from "@/stores/app";

useRecaptchaProvider();
const recaptchaResponse = ref();

const { t } = useLocale();

const authStore = useAuthStore();

const appStore = useAppStore();

const schema = object().shape({
  email: string().email(t("auth.login.email.invalid")).required(t("auth.login.email.required")).label(t("auth.login.email.label")),
  password: string().required(t("auth.login.password.required")).label(t("auth.login.password.label")),
  rememberMe: boolean().default(false).label(t("auth.login.rememberMe"))
});

const { defineField, handleSubmit, resetForm } = useForm({
  validationSchema: schema
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const [email, emailProps] = defineField("email", vuetifyConfig);
const [password, passwordProps] = defineField("password", vuetifyConfig);
const [rememberMe, rememberMeProps] = defineField("rememberMe", vuetifyConfig);

const showPassword = ref(false);

const toaster = createToaster({
  position: "top-right",
  duration: 3000,
  queue: true,
  pauseOnHover: true,
  useDefaultCss: true
});

const onSubmit = handleSubmit(async (values) => {
  appStore.togglePreloader();

  try {
    window.scrollTo({ top: 0, behavior: "smooth" });

    await until(recaptchaResponse).changed();
    await authStore.login(values.email, values.password, values.rememberMe ?? false, recaptchaResponse.value);

    appStore.togglePreloader();
  } catch (error) {
    const errorMessages = error.response.data;
    errorMessages.forEach((message) => toaster.error(message));
    appStore.togglePreloader();
  }
});
</script>

<template>
  <v-row>
    <v-col cols="12" class="text-center">
      <Logo />
      <h2 class="text-secondary text-h2 mt-8">
        {{ t("auth.login.welcome") }}
      </h2>
      <h4 class="text-disabled text-h4 mt-3">
        {{ t("auth.login.subtitle") }}
      </h4>
    </v-col>
  </v-row>

  <v-form class="mt-7 login-form" @submit="onSubmit">
    <v-text-field
      v-model="email"
      v-bind="emailProps"
      type="email"
      :label="t('auth.login.email.label')"
      class="mt-4"
      density="comfortable"
      hide-details="auto"
      variant="outlined"
      color="primary"
    />
    <v-text-field
      v-model="password"
      v-bind="passwordProps"
      :label="t('auth.login.password.label')"
      density="comfortable"
      variant="outlined"
      color="primary"
      hide-details="auto"
      :append-icon="showPassword ? '$eye' : '$eyeOff'"
      :type="showPassword ? 'text' : 'password'"
      @click:append="showPassword = !showPassword"
      class="mt-4 pwd-input"
    />

    <div class="d-sm-flex align-center mt-2 mb-7 mb-sm-0">
      <v-checkbox
        v-model="rememberMe"
        v-bind="rememberMeProps"
        :label="t('auth.login.rememberMe')"
        color="primary"
        class="ms-n2"
        hide-details
      />
      <div class="ml-auto">
        <router-link :to="{ name: 'login', params: { page: 'forgot-password' } }" class="text-primary text-decoration-none">
          {{ t("auth.login.forgotPassword") }}
        </router-link>
      </div>
    </div>

    <div class="d-sm-flex align-center mt-2 mb-7 mb-sm-0">
      <challenge-v3 v-model="recaptchaResponse" action="submit">
        <v-btn color="primary" variant="flat" size="large" type="submit">
          {{ t("auth.login.submit") }}
        </v-btn>
      </challenge-v3>
      <v-btn color="secondary" variant="flat" size="large" class="ml-4" @click="resetForm()">
        {{ t("auth.login.resetForm") }}
      </v-btn>
    </div>
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
.login-form {
  .v-text-field .v-field--active input {
    font-weight: 500;
  }
}
</style>
