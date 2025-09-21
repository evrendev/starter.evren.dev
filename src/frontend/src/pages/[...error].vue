<script setup lang="ts">
const { t } = useI18n();
import misc404 from "@images/pages/404.png";
</script>

<template>
  <div class="misc-wrapper text-center">
    <error-header
      status-code="404"
      :title="t('error.404.title')"
      :description="t('error.404.description')"
    />

    <v-btn :to="{ name: 'home' }" class="mb-6">
      {{ t("error.404.go-back-home") }}
    </v-btn>

    <div class="misc-avatar w-100">
      <v-img
        :src="misc404"
        :alt="t('error.404.image.alt')"
        :max-width="512"
        class="mx-auto"
      />
    </div>
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/admin/template/pages/misc.scss";
</style>
