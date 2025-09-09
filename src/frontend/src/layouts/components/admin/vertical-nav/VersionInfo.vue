<script setup lang="ts">
const { version, buildTime } = useVersion();
</script>

<template>
  <v-row no-gutters>
    <v-col class="d-flex justify-center align-center ga-2" cols="12">
      <v-icon icon="bx-info-circle" size="12" />
      <span class="fs-65"> v: {{ version }} </span>
    </v-col>
  </v-row>
  <v-row no-gutters>
    <v-col class="d-flex justify-center align-center ga-2" cols="12">
      <v-icon icon="bx-calendar" size="12" />
      <span class="fs-65"> Build Time: {{ buildTime }} </span>
    </v-col>
  </v-row>
</template>

<style lang="scss" scoped>
.fs-65 {
  font-size: 0.65rem;
}
</style>
