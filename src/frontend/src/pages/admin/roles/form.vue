<script setup lang="ts">
import { Role } from "@/requests/role";
import { Notify } from "@/stores/notification";
import { RoleForm } from "@/views/admin/roles";

import { useRoleStore } from "@/stores/role";
import { DefaultApiResponse } from "@/responses/api";
const { t } = useI18n();

const roleStore = useRoleStore();
const { loading } = storeToRefs(roleStore);

const route = useRoute();
const router = useRouter();

const pageTitle: ComputedRef<string> = computed(() =>
  t(route.meta.title as string),
);

const role = ref<Role | null>(null);

onMounted(async () => {
  const { id } = route.params;
  if (id) {
    const response: DefaultApiResponse<Role | null> = await roleStore.getById(
      id as string,
    );

    role.value = response?.data ?? null;
  }
});

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { path: "/admin" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.roles"),
    to: { path: "/admin/roles" },
  },
  {
    title: pageTitle.value,
    disabled: true,
  },
]);

const handleSubmit = async (values: Role) => {
  const response: DefaultApiResponse<string> =
    route.name === "role-create"
      ? await roleStore.create(values)
      : await roleStore.update(values);

  if (response.succeeded) {
    Notify.success(
      t(
        `admin.roles.notifications.${route.name === "role-create" ? "created" : "updated"}`,
      ),
    );
    router.push({ name: "tenant-list" });
  } else {
    Notify.error(
      t(
        `admin.roles.notifications.${route.name === "role-create" ? "createFailed" : "updateFailed"}`,
      ),
    );
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <role-form
    :role="role"
    :loading="loading"
    :route-name="route.name"
    :page-title="pageTitle"
    @submit="handleSubmit"
  />
</template>
