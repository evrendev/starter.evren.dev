<script setup lang="ts">
import { toTypedSchema } from "@vee-validate/yup";
import { ref as yupRef, object, string } from "yup";
import { useForm } from "vee-validate";
import { ChangePasswordRequest } from "@/requests/user";
const { t } = useI18n();

const props = defineProps<{
  loading: boolean;
}>();

const schema = toTypedSchema(
  object({
    password: string().required(
      t("admin.personal.security.fields.password.required"),
    ),
    newPassword: string()
      .required(t("admin.personal.security.fields.newPassword.required"))
      .min(8, t("admin.personal.security.fields.newPassword.min", { min: 8 }))
      .matches(
        /^[A-Za-z0-9!@#$%^&*()_+|~\-={}[\]:";<>?,./]+$/,
        t("admin.personal.security.fields.newPassword.special"),
      ),
    confirmNewPassword: string()
      .required(t("admin.personal.security.fields.confirmNewPassword.required"))
      .oneOf(
        [yupRef("newPassword")],
        t("admin.personal.security.fields.confirmNewPassword.match"),
      ),
  }),
);

const { defineField, handleSubmit, resetForm, errors } =
  useForm<ChangePasswordRequest>({
    validationSchema: schema,
  });

const [password, passwordAttrs] = defineField("password");
const [newPassword, newPasswordAttrs] = defineField("newPassword");
const [confirmNewPassword, confirmNewPasswordAttrs] =
  defineField("confirmNewPassword");

const emit = defineEmits<{
  (e: "submit", values: ChangePasswordRequest): void;
}>();

const isCurrentPasswordVisible = ref(false);
const isNewPasswordVisible = ref(false);
const isConfirmPasswordVisible = ref(false);

const submit = handleSubmit((values: ChangePasswordRequest) => {
  emit("submit", values);
  resetForm();
});

const passwordRequirements = [
  t("admin.personal.security.fields.newPassword.min", { min: 8 }),
  t("admin.personal.security.fields.newPassword.lowerUppercase"),
  t("admin.personal.security.fields.newPassword.special"),
];
</script>

<template>
  <v-col cols="12">
    <v-card :title="t('admin.personal.security.changePassword')">
      <v-form>
        <v-card-text>
          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="password"
                v-bind:="passwordAttrs"
                :type="isCurrentPasswordVisible ? 'text' : 'password'"
                :append-inner-icon="
                  isCurrentPasswordVisible ? 'bx-hide' : 'bx-show'
                "
                :label="t('admin.personal.security.fields.password.title')"
                placeholder="············"
                @click:append-inner="
                  isCurrentPasswordVisible = !isCurrentPasswordVisible
                "
                :error-messages="errors.password"
              />
            </v-col>
          </v-row>

          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="newPassword"
                v-bind:="newPasswordAttrs"
                :type="isNewPasswordVisible ? 'text' : 'password'"
                :append-inner-icon="
                  isNewPasswordVisible ? 'bx-hide' : 'bx-show'
                "
                :label="t('admin.personal.security.fields.newPassword.title')"
                autocomplete="on"
                placeholder="············"
                @click:append-inner="
                  isNewPasswordVisible = !isNewPasswordVisible
                "
                :error-messages="errors.newPassword"
              />
            </v-col>

            <v-col cols="12" md="6">
              <v-text-field
                v-model="confirmNewPassword"
                v-bind:="confirmNewPasswordAttrs"
                :type="isConfirmPasswordVisible ? 'text' : 'password'"
                :append-inner-icon="
                  isConfirmPasswordVisible ? 'bx-hide' : 'bx-show'
                "
                :label="
                  t('admin.personal.security.fields.confirmNewPassword.title')
                "
                placeholder="············"
                @click:append-inner="
                  isConfirmPasswordVisible = !isConfirmPasswordVisible
                "
                :error-messages="errors.confirmNewPassword"
              />
            </v-col>
          </v-row>
        </v-card-text>

        <v-card-text>
          <p
            class="text-base font-weight-medium mt-2"
            v-text="t('admin.personal.security.passwordRequirements')"
          />

          <ul class="d-flex flex-column gap-y-3">
            <li v-for="item in passwordRequirements" :key="item" class="d-flex">
              <div>
                <v-icon size="7" icon="bxs-circle" class="me-3" />
              </div>
              <span class="font-weight-medium">{{ item }}</span>
            </li>
          </ul>
        </v-card-text>

        <v-card-text class="d-flex flex-wrap gap-4">
          <v-btn
            color="primary"
            variant="tonal"
            size="small"
            prepend-icon="bx-save"
            @click="submit"
            :loading="props.loading"
          >
            {{ t("shared.save") }}
          </v-btn>

          <v-btn
            type="reset"
            size="small"
            color="secondary"
            variant="tonal"
            prepend-icon="bx-reset"
          >
            {{ t("shared.reset") }}
          </v-btn>
        </v-card-text>
      </v-form>
    </v-card>
  </v-col>
</template>
