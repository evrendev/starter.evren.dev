<script setup>
import { RouterView } from "vue-router";
import { useI18n } from "vue-i18n";
import { computed } from "vue";
import { VSonner } from "vuetify-sonner";
import { Loader, GoToTop } from "@/components/shared";

const { locale } = useI18n();
const key = computed(() => locale.value);
</script>

<template>
  <loader />
  <v-sonner />
  <go-to-top />
  <router-view v-slot="{ Component }">
    <v-fade-transition hide-on-leave>
      <component :is="Component" :key="key" />
    </v-fade-transition>
  </router-view>
</template>
