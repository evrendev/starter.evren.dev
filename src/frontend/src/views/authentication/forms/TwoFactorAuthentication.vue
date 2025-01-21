<script setup>
import Logo from "@/layouts/full/logo/LogoDark.vue";
import { useForm } from "vee-validate";
import { useLocale } from "vuetify";
import { object, string } from "yup";
import { createToaster } from "@meforma/vue-toaster";
import { useAuthStore } from "@/stores/auth";
import { useAppStore } from "@/stores/app";
import { storeToRefs } from "pinia";
import { useRouter } from "vue-router";

const { t } = useLocale();

const authStore = useAuthStore();
const appStore = useAppStore();
const { loading } = storeToRefs(appStore);
const { userId } = storeToRefs(authStore);

const router = useRouter();

if (!userId.value) router.push({ name: "login", params: { page: "login" } });

const schema = object().shape({
  code: string().required(t("auth.TwoFactorAuthentication.required")).label(t("auth.TwoFactorAuthentication.label"))
});

const { defineField, handleSubmit } = useForm({
  validationSchema: schema
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const [code, codeProps] = defineField("code", vuetifyConfig);

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
    await authStore.verify(values.code);

    appStore.togglePreloader();
  } catch (error) {
    const errorMessage = error.response.data;
    toaster.error(errorMessage);
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
        {{ t("auth.TwoFactorAuthentication.subtitle") }}
      </h4>
    </v-col>
  </v-row>

  <v-form class="mt-7 login-form" @submit="onSubmit">
    <v-text-field
      v-model="code"
      v-bind="codeProps"
      type="tel"
      :label="t('auth.TwoFactorAuthentication.label')"
      maxlength="6"
      class="mt-4"
      density="comfortable"
      hide-details="auto"
      variant="outlined"
      color="primary"
      mask="######"
    />

    <div class="d-flex justify-center justify-md-start mt-7 mb-lg-2 mb-sm-0">
      <v-btn color="primary" variant="flat" type="submit" :loading="loading" prepend-icon="$check">
        {{ t("auth.TwoFactorAuthentication.submit") }}
      </v-btn>
    </div>

    <div class="d-flex justify-center mt-7 mb-lg-2 mb-sm-0">
      <router-link :to="{ name: 'login', params: { page: 'login' } }" class="text-primary text-decoration-none" v-show="!loading">
        <v-icon icon="$return" />
        <span class="ml-2">
          {{ t("auth.forgotPassword.back") }}
        </span>
      </router-link>
    </div>
  </v-form>
</template>
<style lang="scss" scoped>
.login-form {
  .v-text-field .v-field--active input {
    font-weight: 500;
  }
}
</style>
