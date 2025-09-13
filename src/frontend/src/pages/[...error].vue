<script setup lang="ts">
import misc404 from "@images/pages/404.png";
</script>

<template>
  <div class="misc-wrapper text-center">
    <error-header
      status-code="404"
      title="Page Not Found ⚠️"
      description="We couldn't find the page you are looking for."
    />

    <v-btn :to="{ name: 'home' }" class="mb-6"> Back to Home </v-btn>

    <div class="misc-avatar w-100">
      <v-img
        :src="misc404"
        alt="Page Not Found"
        :max-width="500"
        class="mx-auto"
      />
    </div>
  </div>
</template>

<style lang="scss">
@use "@/assets/styles/admin/template/pages/misc.scss";
</style>
