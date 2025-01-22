<script setup>
import { RouterView } from "vue-router";
import { useRecaptchaProvider } from "vue-recaptcha";
import { VSonner } from "vuetify-sonner";
useRecaptchaProvider();
</script>

<template>
  <v-sonner />
  <RouterView></RouterView>
</template>
