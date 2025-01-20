<script setup>
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
  rememberMe: boolean().default(false).label(t("auth.login.remember-me"))
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
  <v-form class="mt-7 loginForm" @submit="onSubmit">
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
      class="mt-4 pwdInput"
    />

    <div class="d-sm-flex align-center mt-2 mb-7 mb-sm-0">
      <v-checkbox
        v-model="rememberMe"
        v-bind="rememberMeProps"
        :label="t('auth.login.remember-me')"
        color="primary"
        class="ms-n2"
        hide-details
      />
      <div class="ml-auto">
        <a href="javascript:void(0)" class="text-primary text-decoration-none">
          {{ t("auth.login.forgot-password") }}
        </a>
      </div>
    </div>

    <div class="d-sm-flex align-center mt-2 mb-7 mb-sm-0">
      <challenge-v3 v-model="recaptchaResponse" action="submit">
        <v-btn color="primary" variant="flat" size="large" type="submit">
          {{ t("auth.login.submit") }}
        </v-btn>
      </challenge-v3>
      <v-btn color="secondary" variant="flat" size="large" class="ml-4" @click="resetForm()"> Reset </v-btn>
    </div>
  </v-form>
</template>
<style lang="scss">
.custom-devider {
  border-color: rgba(0, 0, 0, 0.08) !important;
}
.googleBtn {
  border-color: rgba(0, 0, 0, 0.08);
  margin: 30px 0 20px 0;
}
.outlinedInput .v-field {
  border: 1px solid rgba(0, 0, 0, 0.08);
  box-shadow: none;
}
.orbtn {
  padding: 2px 40px;
  border-color: rgba(0, 0, 0, 0.08);
  margin: 20px 15px;
}
.pwdInput {
  position: relative;
  .v-input__append {
    position: absolute;
    right: 10px;
    top: 50%;
    transform: translateY(-50%);
  }
}
.loginForm {
  .v-text-field .v-field--active input {
    font-weight: 500;
  }
}
</style>
