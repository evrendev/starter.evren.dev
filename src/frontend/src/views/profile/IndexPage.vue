<script setup>
import { shallowRef, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { usePredefinedValuesStore } from "@/stores";
import { Breadcrumb } from "@/components/forms";
import { ProfileForm, TwoFactorSection } from "./components";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.profile.title"),
    disabled: true,
    href: "#"
  }
]);

const predefinedValues = usePredefinedValuesStore();

onMounted(async () => {
  await predefinedValues.get();
});
</script>

<template>
  <breadcrumb :title="t('admin.profile.title')" :breadcrumbs="breadcrumbs" />

  <v-row>
    <v-col cols="12" class="mx-auto">
      <v-card class="pa-6">
        <profile-form />

        <v-divider class="my-4" />

        <two-factor-section />
      </v-card>
    </v-col>
  </v-row>
</template>
