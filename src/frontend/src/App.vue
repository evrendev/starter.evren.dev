<script setup>
import { RouterView } from "vue-router";
import { useRecaptchaProvider } from "vue-recaptcha";

// Initialize recaptcha provider once at app root
useRecaptchaProvider();
</script>

<template>
  <RouterView></RouterView>
</template>
