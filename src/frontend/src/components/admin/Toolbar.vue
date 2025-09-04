<script setup lang="ts">
defineProps<{
  title: string;
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
          :prepend-icon="button.icon"
          variant="elevated"
          size="small"
          :text="button.text"
          :to="button.to"
        />
      </template>
    </v-toolbar>
  </v-col>
</template>
