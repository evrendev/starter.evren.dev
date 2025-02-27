<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
import { useUserStore, useAppStore } from "@/stores";
import { Breadcrumb } from "@/components/forms";
import { UserForm } from "./components/";

const { t } = useI18n();
const route = useRoute();
const userStore = useUserStore();
const appStore = useAppStore();
const user = ref(null);

const breadcrumbs = ref([
  {
    title: t("admin.users.title"),
    disabled: false,
    href: "/admin/users/list"
  },
  {
    title: t("admin.users.edit"),
    disabled: true,
    href: "#"
  }
]);

onMounted(async () => {
  const id = route.params.id;

  try {
    appStore.setPageLoading(true);
    await userStore.getById(id);
    user.value = userStore.user;
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoading(false);
  }
});
</script>

<template>
  <breadcrumb :title="t('admin.users.new')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <user-form :is-edit="true" />
    </v-col>
  </v-row>
</template>
