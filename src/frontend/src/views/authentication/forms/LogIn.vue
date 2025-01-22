<script setup>
import Logo from "@/layouts/full/logo/LogoDark.vue";
import { ref } from "vue";
import { useLocale } from "vuetify";
import { useForm } from "vee-validate";
import { object, string, boolean } from "yup";
import { ChallengeV3 } from "vue-recaptcha";
import { until } from "@vueuse/core";
import { useAuthStore, useAppStore } from "@/stores/";
import { storeToRefs } from "pinia";

const { t } = useLocale();

const authStore = useAuthStore();
const appStore = useAppStore();
const { loading } = storeToRefs(appStore);

const schema = object().shape({
  email: string().email(t("auth.login.email.invalid")).required(t("auth.login.email.required")).label(t("auth.login.email.label")),
  password: string().required(t("auth.login.password.required")).label(t("auth.login.password.label")),
  rememberMe: boolean().default(false).label(t("auth.login.rememberMe")),
  response: string().required(t("auth.login.recaptcha.required"))
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
const [response] = defineField("response", vuetifyConfig);

const showPassword = ref(false);

const onSubmit = handleSubmit(async (values) => {
  try {
    window.scrollTo({ top: 0, behavior: "smooth" });
    appStore.setPageLoader(true);

    await until(response).changed();
    await authStore.login(values);
  } catch (error) {
    resetForm();
    console.error(error);
    appStore.setPageLoader(false);
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

  <v-form class="mt-7 login-form" :disabled="loading" @submit="onSubmit">
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
        <router-link
          :to="{ name: 'login', params: { page: 'forgot-password' } }"
          class="text-primary text-decoration-none"
          v-show="!loading"
        >
          <v-icon icon="$fish" />
          <span class="ml-2">
            {{ t("auth.login.forgotPassword") }}
          </span>
        </router-link>
      </div>
    </div>

    <div class="d-flex justify-center justify-md-start mt-7 mb-lg-2 mb-sm-0">
      <challenge-v3 v-model="response" action="submit">
        <v-btn color="primary" variant="flat" type="submit" :loading="loading" prepend-icon="$login">
          {{ t("auth.login.submit") }}
        </v-btn>
      </challenge-v3>
      <v-btn color="secondary" variant="flat" class="ml-4" @click="resetForm()" prepend-icon="$refresh" :disabled="loading">
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
