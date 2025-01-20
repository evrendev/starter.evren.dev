<script setup>
import { useForm } from "vee-validate"
import { useLocale } from "vuetify"
import { object, string } from "yup"
import { createToaster } from "@meforma/vue-toaster"
import { useAuthStore } from "@/stores/auth"
import { useAppStore } from "@/stores/app"

const { t } = useLocale()

const authStore = useAuthStore()

const appStore = useAppStore()

const schema = object().shape({
  code: string()
    .required(t("auth.login.code.required"))
    .label(t("auth.login.code.label")),
})

const { defineField, handleSubmit } = useForm({
  validationSchema: schema,
})

const vuetifyConfig = state => ({
  props: {
    "error-messages": state.errors,
  },
})

const [code, codeProps] = defineField("code", vuetifyConfig)

const toaster = createToaster({
  position: "top-right",
  duration: 3000,
  queue: true,
  pauseOnHover: true,
  useDefaultCss: true,
})

const onSubmit = handleSubmit(async values => {
  appStore.togglePreloader()

  try {
    await authStore.verify(values.code)

    appStore.togglePreloader()
  } catch (error) {
    const errorMessages = error.response.data
    errorMessages.forEach(message => toaster.error(message))
    appStore.togglePreloader()
  }
})
</script>

<template>
  <v-form class="mt-7 loginForm" @submit="onSubmit">
    <v-text-field
      v-model="code"
      v-bind="codeProps"
      type="text"
      :label="t('auth.login.code.label')"
      maxlength="6"
      class="mt-4"
      density="comfortable"
      hide-details="auto"
      variant="outlined"
      color="primary"
    />

    <div class="d-sm-flex align-center mt-2 mb-7 mb-sm-0">
      <v-btn color="primary" variant="flat" size="large" type="submit">
        {{ t("auth.login.submit") }}
      </v-btn>
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
