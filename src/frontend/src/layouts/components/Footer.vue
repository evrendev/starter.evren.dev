<script lang="ts" setup>
const { t } = useI18n();
</script>

<template>
  <div
    class="h-100 d-flex align-center justify-md-space-between justify-center"
  >
    <span class="d-flex align-center">
      &copy;
      {{ new Date().getFullYear() }}
      Made with
      <VIcon icon="bx-heart" color="error" size="1rem" class="mx-1" />
      in Delmenhorst
    </span>

    <span class="d-md-flex gap-x-4 text-primary d-none">
      <router-link :to="{ name: 'home' }">
        {{ t("admin.components.footer.home") }}
      </router-link>
      <a href="https://evren.dev/panel/documentation" target="_blank">
        {{ t("admin.components.footer.documentation") }}
      </a>
      <a href="https://evren.dev/panel/support" target="_blank">
        {{ t("admin.components.footer.support") }}
      </a>
    </span>
  </div>
</template>
