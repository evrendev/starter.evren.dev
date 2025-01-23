<script setup>
import { RouterView } from "vue-router";
import { useRecaptchaProvider } from "vue-recaptcha";
import { useI18n } from "vue-i18n";
import { computed } from "vue";
import { VSonner } from "vuetify-sonner";

const { locale } = useI18n();
const key = computed(() => locale.value);
useRecaptchaProvider();
</script>

<template>
  <v-sonner />
  <RouterView v-slot="{ Component }">
    <v-fade-transition mode="out-in">
      <component :is="Component" :key="key" />
    </v-fade-transition>
  </RouterView>
</template>
