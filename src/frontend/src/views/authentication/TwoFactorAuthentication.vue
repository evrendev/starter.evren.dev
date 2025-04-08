<script setup>
import { useForm } from "vee-validate";
import { useLocale } from "vuetify";
import { object, string } from "yup";
import { useAuthStore } from "@/stores/auth";
import { useAppStore } from "@/stores/app";
import { storeToRefs } from "pinia";
import { useRouter } from "vue-router";
import { Logo } from "@/layouts/full/logo/";

const { t } = useLocale();

const authStore = useAuthStore();
const appStore = useAppStore();
const { loading } = storeToRefs(appStore);
const { user } = storeToRefs(authStore);

const router = useRouter();

if (!user.value.id) router.push({ name: "login", query: { returnUrl: null } });

const schema = object().shape({
  code: string()
    .required(t("auth.twoFactorAuth.required"))
    .label(t("auth.twoFactorAuth.label"))
    .matches(/^[0-9]{6}$/, t("auth.twoFactorAuth.code.invalid"))
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

const onSubmit = handleSubmit(async (values) => {
  try {
    appStore.setLoading(true);
    await authStore.verify({ userId: user.value.id, code: values.code, rememberMachine: true });
  } catch (error) {
    console.error(error);
    appStore.setLoading(false);
  }
});
</script>

<template>
  <v-row>
    <v-col cols="12" class="text-center">
      <logo />
      <h2 class="text-secondary text-h2 mt-8">
        {{ t("auth.login.welcome") }}
      </h2>
      <h4 class="text-disabled text-h4 mt-3">
        {{ t("auth.twoFactorAuth.subtitle") }}
      </h4>
    </v-col>
  </v-row>

  <v-form class="mt-7 login-form" @submit="onSubmit">
    <v-text-field
      v-model="code"
      v-bind="codeProps"
      type="tel"
      :label="t('auth.twoFactorAuth.label')"
      maxlength="6"
      class="mt-4"
      density="compact"
      hide-details="auto"
      variant="outlined"
      color="primary"
      mask="######"
    />

    <div class="d-flex justify-center justify-md-start mt-7 mb-lg-2 mb-sm-0">
      <v-btn color="primary" variant="flat" type="submit" :loading="loading" prepend-icon="$check">
        {{ t("auth.twoFactorAuth.submit") }}
      </v-btn>
    </div>

    <div class="d-flex justify-center mt-7 mb-lg-2 mb-sm-0">
      <router-link :to="{ name: 'login' }" class="text-primary text-decoration-none" v-show="!loading">
        <v-icon icon="$return" />
        <span class="ml-2">
          {{ t("auth.forgotPassword.back") }}
        </span>
      </router-link>
    </div>
  </v-form>
</template>
