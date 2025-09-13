<script setup lang="ts">
import { ref } from "vue";
const { t } = useI18n();

const breadcrumbs = ref([
  { title: t("admin.components.breadcrumbs.home"), to: { path: "/" } },
  {
    title: t("admin.components.breadcrumbs.dashboard"),
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
