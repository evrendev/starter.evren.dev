<script setup lang="ts">
defineProps<{
  items: Array<{
    title: string;
    to?: { path: string } | { name: string };
    disabled?: boolean;
  }>;
}>();
</script>

<template>
  <v-row>
    <v-col>
      <v-breadcrumbs :items="items" class="border rounded" bg-color="primary">
        <template v-slot:prepend>
          <v-icon icon="bx-home" size="small" />

          <v-icon icon="bx-chevron-right" />
        </template>
        <template v-slot:divider>
          <v-icon icon="bx-chevron-right" />
        </template>
      </v-breadcrumbs>
    </v-col>
  </v-row>
</template>
