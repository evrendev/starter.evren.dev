<script setup lang="ts">
const { t } = useI18n();
</script>

<template>
  <div class="home">
    <h1 class="text-h2 text-center my-12">{{ t("home.welcome_message") }}</h1>
    <p class="text-subtitle-1 text-center">{{ t("home.intro_text") }}</p>
  </div>
</template>
