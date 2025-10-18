<script setup lang="ts">
import { Notify } from "@/stores/notification";
import { Permissions } from "@/views/admin/roles";

import { useRoleStore } from "@/stores/role";
import { Result } from "@/primitives/result";
import { Role } from "@/models/role";
const { t } = useI18n();

const roleStore = useRoleStore();
const { loading } = storeToRefs(roleStore);

const route = useRoute();
const router = useRouter();

const role = ref<Role | null>(null);

onMounted(async () => {
  const { id } = route.params;
  if (id) {
    const response: Result<Role | null> = await roleStore.getRolePermissions(
      id as string,
    );

    if (!response.succeeded) router.push({ name: "role-list" });

    role.value = response?.data ?? null;
  } else {
    router.push({ name: "role-list" });
  }
});

const pageTitle: ComputedRef<string> = computed(() =>
  t(`admin.roles.permissions.title`, {
    roleName: role.value?.name ?? "",
  }),
);

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

const handleSubmit = async (values: Pick<Role, "permissions">) => {
  role.value!.permissions = values.permissions;

  const response: Result<string> = await roleStore.updateRolePermissions(
    role.value!,
  );

  if (response.succeeded) {
    Notify.success(t(`admin.roles.notifications.updatePermissions`));

    router.push({ name: "role-list" });
  } else {
    Notify.error(t(`admin.roles.notifications.updatePermissionsFailed`));
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <permissions
    :loading="loading"
    :role="role"
    :page-title="pageTitle"
    :route-name="route.name"
    @submit="handleSubmit"
  />
</template>
