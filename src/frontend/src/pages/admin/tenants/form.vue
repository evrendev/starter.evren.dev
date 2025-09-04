<script setup lang="ts">
import { Tenant } from "@/requests/tenant";
import { Notify } from "@/stores/notification";
import breadcrumb from "@/components/admin/breadcrumb.vue";
import TenantForm from "@/views/admin/tenants/Form.vue";

import { useTenantStore } from "@/stores/tenant";
import { DefaultApiResponse } from "@/responses/api";
const { t } = useI18n();

const tenantStore = useTenantStore();
const { loading } = storeToRefs(tenantStore);

const route = useRoute();
const router = useRouter();
const viewOnly = ref(route.name === "tenants-view");

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

watch(
  () => route.name,
  (routeName) => {
    viewOnly.value = routeName === "tenants-view";
  },
);

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
  const response: DefaultApiResponse<string> = await tenantStore.update(values);

  if (response.succeeded) {
    Notify.success(t("admin.tenants.notifications.updated"));
    router.push({ name: "tenants-list" });
  } else {
    Notify.error(t("admin.tenants.notifications.updateFailed"));
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <tenant-form
    :tenant="tenant"
    :loading="loading"
    :view-only="viewOnly"
    :page-title="pageTitle"
    @enable-edit="viewOnly = false"
    @submit="handleSubmit"
  />
</template>
