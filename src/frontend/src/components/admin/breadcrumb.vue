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
        <template v-slot:divider>
          <v-icon icon="bx-chevron-right"></v-icon>
        </template>
      </v-breadcrumbs>
    </v-col>
  </v-row>
</template>
