<script setup>
import { ref } from "vue";
import { useLocale } from "vuetify";
import { useForm } from "vee-validate";
import { object, string, boolean } from "yup";
import { useAuthStore, useAppStore } from "@/stores/";
import { storeToRefs } from "pinia";
import { RecaptchaButton } from "@/components/forms";
import { Logo } from "@/layouts/full/logo";

const { t } = useLocale();

const authStore = useAuthStore();
const appStore = useAppStore();
const { loading } = storeToRefs(appStore);

// reCAPTCHA site key
const siteKey = ref(import.meta.env.VITE_RECAPTCHA_SITE_KEY_V3 || "");

// Form schema
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
const recaptchaToken = ref("");

// Handle reCAPTCHA success
const handleRecaptchaSuccess = (token) => {
  recaptchaToken.value = token;
  submitForm();
};

// Handle reCAPTCHA error
const handleRecaptchaError = (error) => {
  console.error("reCAPTCHA error:", error);
  appStore.setLoading(false);
};

// Prepare and validate form
const onSubmit = handleSubmit(() => {
  window.scrollTo({ top: 0, behavior: "smooth" });
  appStore.setLoading(true);
  // Form validation passed, reCAPTCHA will be triggered by the button component
});

// Submit form with token
const submitForm = async () => {
  try {
    if (!recaptchaToken.value) {
      console.error("No reCAPTCHA token available");
      appStore.setLoading(false);
      return;
    }

    // Prepare login data with form values and reCAPTCHA token
    const values = {
      email: email.value,
      password: password.value,
      rememberMe: rememberMe.value,
      response: recaptchaToken.value
    };

    // Send login request
    await authStore.login(values);
  } catch (error) {
    console.error("Login error:", error);
    resetForm();
    appStore.setLoading(false);
  }
};
</script>

<template>
  <v-row>
    <v-col cols="12" class="text-center">
      <logo />
      <h2 class="text-secondary text-h2 mt-8">
        {{ t("auth.login.welcome") }}
      </h2>
      <h4 class="text-disabled text-h4 mt-3">
        {{ t("auth.login.subtitle") }}
      </h4>
    </v-col>
  </v-row>

  <v-form class="mt-7 login-form" :disabled="loading" @submit.prevent="onSubmit">
    <v-text-field
      v-model="email"
      v-bind="emailProps"
      :label="t('auth.login.email.label')"
      type="email"
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
      :append-icon="showPassword ? '$eye' : '$eyeOff'"
      :type="showPassword ? 'text' : 'password'"
      @click:append="showPassword = !showPassword"
      density="comfortable"
      variant="outlined"
      color="primary"
      hide-details="auto"
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
        <router-link :to="{ name: 'forgot-password' }" class="text-primary text-decoration-none" v-show="!loading">
          <v-icon icon="$fish" />
          <span class="ml-2">
            {{ t("auth.login.forgotPassword") }}
          </span>
        </router-link>
      </div>
    </div>

    <div class="d-flex justify-center justify-md-start mt-7 mb-lg-2 mb-sm-0">
      <v-btn color="secondary" variant="flat" class="mr-4" @click="resetForm()" prepend-icon="$refresh" :disabled="loading">
        {{ t("auth.login.resetForm") }}
      </v-btn>

      <recaptcha-button
        button-text="Login"
        button-color="primary"
        button-variant="flat"
        button-icon="$login"
        :loading="loading"
        :site-key="siteKey"
        action="login"
        @recaptcha-success="handleRecaptchaSuccess"
        @recaptcha-error="handleRecaptchaError"
      />
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
