<script setup lang="ts">
const { t } = useI18n();
</script>

<template>
  <div>
    <h1>
      {{ t("app.title") }}
    </h1>
    <h4>
      {{ t("app.description") }}
    </h4>
  </div>
</template>
