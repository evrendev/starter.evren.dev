<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { Breadcrumb } from "@/components/forms";
import { UserForm } from "./components/";

const { t } = useI18n();

const breadcrumbs = ref([
  {
    title: t("admin.users.title"),
    disabled: false,
    href: "/admin/users/list"
  },
  {
    title: t("admin.users.new"),
    disabled: true,
    href: "#"
  }
]);
</script>

<template>
  <breadcrumb :title="t('admin.users.new')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <user-form :is-edit="false" />
    </v-col>
  </v-row>
</template>
