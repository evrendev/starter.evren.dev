<script setup>
import { useForm } from "vee-validate";
import { useLocale } from "vuetify";
import { object, string } from "yup";
import { useAuthStore } from "@/stores/auth";
import { useAppStore } from "@/stores/app";
import { Logo } from "@/layouts/full/logo";

const { t } = useLocale();

const authStore = useAuthStore();
const appStore = useAppStore();

const schema = object().shape({
  email: string().required(t("auth.forgotPassword.email.required")).label(t("auth.forgotPassword.email.label"))
});

const { defineField, handleSubmit } = useForm({
  validationSchema: schema
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const [email, emailProps] = defineField("email", vuetifyConfig);

const onSubmit = handleSubmit(async (values) => {
  try {
    appStore.setLoading(true);
    await authStore.forgotPassword(values.email);
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
        {{ t("auth.forgotPassword.welcome") }}
      </h2>
      <h4 class="text-disabled text-h4 mt-3">
        {{ t("auth.forgotPassword.subtitle") }}
      </h4>
    </v-col>
  </v-row>

  <v-form class="mt-7 forgot-password-form" @submit="onSubmit" v-slot="{ isSubmitting }">
    <v-text-field
      v-model="email"
      v-bind="emailProps"
      type="text"
      :label="t('auth.forgotPassword.email.label')"
      maxlength="6"
      class="mt-4"
      density="comfortable"
      hide-details="auto"
      variant="outlined"
      color="primary"
    />

    <div class="d-flex justify-center justify-md-start mt-7 mb-lg-2 mb-sm-0">
      <v-btn color="primary" variant="flat" type="submit" :loading="isSubmitting" prepend-icon="$refresh">
        {{ t("auth.forgotPassword.submit") }}
      </v-btn>
    </div>

    <div class="d-flex justify-center mt-7 mb-lg-2 mb-sm-0">
      <router-link :to="{ name: 'login' }" class="text-primary text-decoration-none">
        <v-icon icon="$return" />
        <span class="ml-2">
          {{ t("auth.forgotPassword.back") }}
        </span>
      </router-link>
    </div>
  </v-form>
</template>

<style lang="scss" scoped>
.forgot-password-form {
  .v-text-field .v-field--active input {
    font-weight: 500;
  }
}
</style>
