<script setup>
const props = defineProps({
  title: String,
  actionButton: {
    type: Object
  }
});
</script>

// ===============================|| Ui Parent Card||=============================== //
<template>
  <v-card variant="flat">
    <v-card-item>
      <div class="d-sm-flex align-center justify-space-between px-2 py-2">
        <v-card-title>{{ props.title }}</v-card-title>
        <slot name="action" v-if="actionButton">
          <v-btn
            icon
            :color="actionButton.color"
            :loading="actionButton.loading"
            @click="actionButton.onClick"
            :size="actionButton.size"
            :disabled="actionButton.disabled"
          >
            <v-icon :icon="actionButton.icon" />
            <v-tooltip activator="parent" location="top" :text="actionButton.text" />
          </v-btn>
        </slot>
      </div>
    </v-card-item>
    <v-divider></v-divider>
    <v-card-text>
      <slot />
    </v-card-text>
  </v-card>
</template>
