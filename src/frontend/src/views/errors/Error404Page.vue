<script setup>
import { useI18n } from "vue-i18n";
import Background from "@/assets/images/error/bg.svg?raw";
import Blue from "@/assets/images/error/blue.svg?raw";
import Purple from "@/assets/images/error/purple.svg?raw";
import Text from "@/assets/images/error/text.svg?raw";

const { t } = useI18n();
</script>

<template>
  <v-row no-gutters class="h-screen">
    <v-col class="d-flex align-center justify-center">
      <div class="text-center">
        <div class="card-media-wrapper">
          <div class="w-100" v-html="Background" />
          <div class="card-media-parts" v-html="Blue" />
          <div class="card-media-build" v-html="Text" />
          <div class="card-media-build" v-html="Purple" />
        </div>
        <h1 class="text-h1" v-text="t('error.title')" />
        <p>
          <small class="error-description" v-html="t('error.description')" />
        </p>
        <v-btn variant="flat" color="primary" class="mt-4" :to="{ name: 'home' }" prepend-icon="$home">
          {{ t("error.home") }}
        </v-btn>
      </div>
    </v-col>
  </v-row>
</template>

<style lang="scss">
.card-media-wrapper {
  max-width: 720px;
  margin: 0 auto;
  position: relative;

  .card-media-build {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    animation: 5s bounce ease-in-out infinite;
  }

  .error-description {
    white-space: pre-line;
    display: inline-block;
    max-width: 80%;
    margin: 0 auto;
  }
}
</style>
