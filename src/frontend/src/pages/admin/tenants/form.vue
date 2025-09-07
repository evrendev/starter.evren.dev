<script setup lang="ts">
import { Tenant } from "@/requests/tenant";
import { Notify } from "@/stores/notification";
import { TenantForm } from "@/views/admin/tenants";

import { useTenantStore } from "@/stores/tenant";
import { DefaultApiResponse } from "@/responses/api";
const { t } = useI18n();

const tenantStore = useTenantStore();
const { loading } = storeToRefs(tenantStore);

const route = useRoute();
const router = useRouter();

const pageTitle: ComputedRef<string> = computed(() =>
  t(route.meta.title as string),
);

const tenant = ref<Tenant | null>(null);

onMounted(async () => {
  const { id } = route.params;
  if (id) {
    tenant.value = await tenantStore.getTenant(id as string);
  }
});

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { path: "/admin" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.tenants"),
    to: { path: "/admin/tenants" },
  },
  {
    title: pageTitle.value,
    disabled: true,
  },
]);

const handleSubmit = async (values: Tenant) => {
  const response: DefaultApiResponse<string> =
    route.name === "tenant-create"
      ? await tenantStore.create(values)
      : await tenantStore.update(values);

  if (response.succeeded) {
    Notify.success(
      t(
        `admin.tenants.notifications.${route.name === "tenant-create" ? "created" : "updated"}`,
      ),
    );
    router.push({ name: "tenant-list" });
  } else {
    Notify.error(
      t(
        `admin.tenants.notifications.${route.name === "tenant-create" ? "createFailed" : "updateFailed"}`,
      ),
    );
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <tenant-form
    :tenant="tenant"
    :loading="loading"
    :route-name="route.name"
    :page-title="pageTitle"
    @submit="handleSubmit"
  />
</template>
