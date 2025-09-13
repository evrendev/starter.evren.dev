<script setup lang="ts">
import { useForm } from "vee-validate";
import { object, string } from "yup";
import QRCode from "qrcode";
import { EnableTwoFactorAuthenticationRequest } from "@/requests/personal";
import {
  RecoverCodesResponse,
  SetupTwoFactorAuthenticationResponse,
} from "@/responses/personal";

const { t } = useI18n();

const props = defineProps<{
  twoFactorEnabled: boolean;
  setupData: SetupTwoFactorAuthenticationResponse;
  recoverData: RecoverCodesResponse;
  loading: boolean;
}>();

const schema = object().shape({
  code: string()
    .required(t("admin.personal.security.fields.twoFactorCode.required"))
    .matches(
      /^[0-9]{6}$/,
      t("admin.personal.security.fields.twoFactorCode.invalid"),
    ),
});

const { defineField, handleSubmit, errors } =
  useForm<EnableTwoFactorAuthenticationRequest>({
    validationSchema: schema,
  });

const [code, codeAttrs] = defineField("code");

const qrCodeImage = ref<string | null>(null);
const confirmDisable2FADialog = ref(false);

const emit = defineEmits<{
  (
    e: "enable-two-factor-authentication",
    values: EnableTwoFactorAuthenticationRequest,
  ): void;
  (e: "disable-two-factor-authentication"): void;
  (e: "setup-two-factor-authentication"): void;
}>();

const enableTwoFactorAuthentication = handleSubmit(
  (values: EnableTwoFactorAuthenticationRequest) => {
    emit("enable-two-factor-authentication", values);
  },
);

const disableTwoFactorAuthentication = () => {
  emit("disable-two-factor-authentication");
};

const setupTwoFactorAuthentication = () => {
  emit("setup-two-factor-authentication");
};

watch(
  () => props.setupData.qrCodeUri,
  async (uri) => {
    if (uri) {
      qrCodeImage.value = await QRCode.toDataURL(uri, {
        width: 160,
        margin: 2,
        color: {
          dark: "#000",
          light: "#FFF",
        },
      });
    }
  },
  { immediate: true },
);
</script>
<template>
  <v-col cols="12">
    <v-card :title="t('admin.personal.security.2FA.title')">
      <v-card-text>
        <p
          class="font-weight-semibold"
          v-text="
            t(
              `admin.personal.security.2FA.${twoFactorEnabled ? 'enabled' : 'notEnabled'}`,
            )
          "
        />
        <p v-text="t('admin.personal.security.2FA.description')" />

        <v-btn
          v-if="twoFactorEnabled"
          color="error"
          variant="elevated"
          prepend-icon="bx-lock-open"
          size="small"
          @click="confirmDisable2FADialog = true"
          :loading="loading"
        >
          {{ t("admin.personal.security.2FA.disable") }}
        </v-btn>

        <v-btn
          v-else
          color="primary"
          size="small"
          prepend-icon="bx-lock-alt"
          @click="setupTwoFactorAuthentication"
          :loading="loading"
        >
          {{ t("admin.personal.security.2FA.enable") }}
        </v-btn>
      </v-card-text>
    </v-card>
  </v-col>

  <modal-window
    :show-modal="recoverData.showRecoverCodes"
    :title="t('admin.personal.security.2FA.setupTitle')"
  >
    <template #content>
      <v-card class="pa-2">
        <v-row no-gutters>
          <v-col class="text-center">
            <h4
              class="font-weight-semibold"
              v-text="t('admin.personal.security.2FA.recoverCodes')"
            />
            <p v-text="t('admin.personal.security.2FA.recoverDescription')" />
          </v-col>
        </v-row>
        <v-card-text class="text-center">
          <code style="white-space: pre-wrap; word-break: break-all"
            >{{ recoverData.data?.join("\n") }}
          </code>
        </v-card-text>
      </v-card>
    </template>

    <template #action-buttons>
      <v-btn
        color="primary"
        variant="elevated"
        size="small"
        @click="recoverData.showRecoverCodes = false"
      >
        {{ t("shared.close") }}
      </v-btn>
    </template>
  </modal-window>
  <modal-window
    :show-modal="setupData.showSetup"
    :title="t('admin.personal.security.2FA.setupTitle')"
  >
    <template #content>
      <v-row>
        <v-col col="12" class="d-flex align-center justify-center">
          <img
            v-if="qrCodeImage"
            :src="qrCodeImage"
            alt="QR Code"
            width="160"
            height="160"
            class="border rounded"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" class="text-center">
          <p
            class="font-weight-semibold"
            v-text="
              `${t('admin.personal.security.2FA.manualSetup', { key: setupData?.sharedKey })}:`
            "
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" class="text-center">
          <label
            class="font-weight-semibold"
            v-text="t('admin.personal.security.2FA.enterCode')"
          />
          <v-otp-input
            autofocus
            v-model="code"
            v-bind="codeAttrs"
            :error-messages="errors.code"
          />
        </v-col>
      </v-row>
    </template>
    <template #action-buttons>
      <v-btn
        color="primary"
        variant="elevated"
        :loading="loading"
        size="small"
        @click="enableTwoFactorAuthentication"
      >
        {{ t("admin.personal.security.2FA.complete") }}
      </v-btn>
      <v-btn
        text
        :disabled="loading"
        @click="setupData.showSetup = false"
        size="small"
      >
        {{ t("shared.cancel") }}
      </v-btn>
    </template>
  </modal-window>

  <confirm-dialog
    v-model:show-dialog="confirmDisable2FADialog"
    :confirm-button-text="t('shared.confirm')"
    :cancel-button-text="t('shared.cancel')"
    :title="t('admin.personal.security.2FA.disableConfirmTitle')"
    :message="t('admin.personal.security.2FA.disableConfirmMessage')"
    @confirm="disableTwoFactorAuthentication"
    @cancel="confirmDisable2FADialog = false"
  />
</template>
