<script setup lang="ts">
import { useTenantStore } from "@/stores/tenant";
const { t } = useI18n();
const tenantStore = useTenantStore();
const filters = tenantStore.filters;

const { itemsPerPage, items, total, loading } = storeToRefs(tenantStore);

onMounted(async () => {
  await tenantStore.getItems(filters);
});

const breadcrumbs = ref([
  {
    title: t("admin.components.breadcrumbs.home"),
    to: { path: "/" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    disabled: true,
  },
  {
    title: t("admin.components.breadcrumbs.admin.tenants"),
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
  <v-row>
    <v-col cols="12">
      <v-card class="mx-auto" elevation="4">
        <v-card-item>
          <v-card-title>Influencing The Influencer</v-card-title>
        </v-card-item>

        <v-card-text>
          <v-data-table-server
            :items-per-page="itemsPerPage"
            :items="items"
            :items-length="total"
            :loading="loading"
            class="striped"
            item-value="id"
          />
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>
