<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import { useUserStore } from "@/stores/user";
import Logo from "@/components/shared/Logo.vue";
import authV1BottomShape from "@images/svg/auth-v1-bottom-shape.svg?url";
import authV1TopShape from "@images/svg/auth-v1-top-shape.svg?url";

const userStore = useUserStore();
const route = useRoute();

const { t } = useI18n();

type Status = "loading" | "success" | "error";
const status = ref<Status>("loading");

onMounted(async () => {
  const tenant = String(route.query.tenant || "");
  const userId = String(route.query.userId || "");
  const code = String(route.query.code || "");

  if (!tenant || !userId || !code) {
    status.value = "error";
    return;
  }

  try {
    await userStore.confirmEmail(tenant, userId, code);
    status.value = "success";
  } catch {
    status.value = "error";
  }
});
</script>

<template>
  <div class="auth-wrapper d-flex align-center justify-center pa-4">
    <div class="position-relative my-sm-16">
      <v-img
        :src="authV1TopShape"
        class="text-primary auth-v1-top-shape d-none d-sm-block"
      />
      <v-img
        :src="authV1BottomShape"
        class="text-primary auth-v1-bottom-shape d-none d-sm-block"
      />

      <v-card
        class="auth-card"
        max-width="460"
        :class="$vuetify.display.smAndUp ? 'pa-6' : 'pa-0'"
      >
        <v-card-item class="justify-center">
          <logo />
        </v-card-item>

        <v-card-text>
          <h4 class="text-h4 mb-1 text-center">
            {{ t("auth.confirmEmail.title") }}
          </h4>
        </v-card-text>

        <v-card-text class="text-center">
          <div v-if="status === 'loading'">
            <v-progress-circular indeterminate color="primary" class="mb-4" />
            <p>{{ t("auth.confirmEmail.loading") }}</p>
          </div>

          <div v-else-if="status === 'success'">
            <v-alert type="success" density="compact" class="mb-4">
              {{ t("auth.confirmEmail.success") }}
            </v-alert>
            <v-btn color="primary" variant="flat" :block="true" :to="{ name: 'login' }">
              {{ t("auth.confirmEmail.backToLogin") }}
            </v-btn>
          </div>

          <div v-else>
            <v-alert type="error" density="compact" class="mb-4">
              {{ t("auth.confirmEmail.error") }}
            </v-alert>
            <v-btn color="primary" variant="flat" :block="true" :to="{ name: 'login' }">
              {{ t("auth.confirmEmail.backToLogin") }}
            </v-btn>
          </div>
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/admin/template/pages/page-auth";
</style>
