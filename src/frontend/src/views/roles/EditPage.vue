<script setup>
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { useRoleStore } from "@/stores/roles";
import { useAppStore } from "@/stores/app";
import { Breadcrumb } from "@/components/forms";
import { RoleForm } from "./components";

const { t } = useI18n();
const route = useRoute();
const roleStore = useRoleStore();
const appStore = useAppStore();
const role = ref(null);

const breadcrumbs = ref([
  {
    title: t("admin.roles.title"),
    disabled: false,
    href: "/admin/roles/list"
  },
  {
    title: t("admin.roles.edit"),
    disabled: true,
    href: "#"
  }
]);

onMounted(async () => {
  const id = route.params.id;

  try {
    appStore.setLoading(true);
    await roleStore.getById(id);
    role.value = roleStore.role;
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setLoading(false);
  }
});
</script>

<template>
  <breadcrumb :title="t('admin.roles.edit')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col cols="12" md="12">
      <role-form v-if="role" :initial-data="role" :is-edit="true" />
    </v-col>
  </v-row>
</template>
