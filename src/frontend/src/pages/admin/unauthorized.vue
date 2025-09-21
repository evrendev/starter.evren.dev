<script setup lang="ts">
import { useI18n } from "vue-i18n";
import misc401 from "@images/pages/401.png";

const { t } = useI18n();
</script>

<template>
  <div class="misc-wrapper text-center">
    <error-header
      status-code="401"
      :title="t('error.401.title')"
      :description="t('error.401.description')"
    />

    <v-btn :to="{ name: 'dashboard' }" class="mb-6">
      {{ t("error.401.go-back-dashboard") }}
    </v-btn>

    <div class="misc-avatar w-100">
      <v-img
        :src="misc401"
        :alt="t('error.401.image.alt')"
        :max-width="512"
        class="mx-auto"
      />
    </div>
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/admin/template/pages/misc.scss";
</style>
