<script setup lang="ts">
import { useTenantStore } from "@/stores/tenant";
const { t } = useI18n();

const tenantStore = useTenantStore();
const { tenant, loading } = storeToRefs(tenantStore);

const route = useRoute();

onMounted(async () => {
  const { id } = route.params;
  await tenantStore.getTenant(id as string);
});

const breadcrumbs = ref([
  {
    title: t("admin.components.breadcrumbs.home"),
    to: { path: "/" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { path: "/admin" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.tenants"),
    to: { path: "/admin/tenants" },
  },
  {
    title: t("shared.edit"),
    disabled: true,
  },
]);
</script>
<template>
  <v-row>
    <v-col>
      <v-breadcrumbs
        :items="breadcrumbs"
        class="border rounded"
        bg-color="primary"
      >
        <template v-slot:divider>
          <v-icon icon="bx-chevron-right"></v-icon>
        </template>
      </v-breadcrumbs>
    </v-col>
  </v-row>
</template>
