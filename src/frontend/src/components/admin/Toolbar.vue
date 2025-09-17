<script setup lang="ts">
defineProps<{
  title: string;
  color: "success" | "primary" | "info" | "warning" | "error";
  button: {
    icon: string;
    text: string;
    to: { path: string } | { name: string };
  };
  disabled?: boolean;
}>();
</script>

<template>
  <v-col>
    <v-toolbar :title="title" density="compact" color="surface">
      <template v-slot:append>
        <v-btn
          variant="flat"
          size="small"
          :color="color"
          :prepend-icon="button.icon"
          :text="button.text"
          :to="button.to"
        />
      </template>
    </v-toolbar>
  </v-col>
</template>
