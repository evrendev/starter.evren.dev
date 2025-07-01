<script setup lang="ts">
const theme = useTheme()
provide(computed(() => (theme.current.value.dark ? 'dark' : undefined)))
</script>

<template>
  <v-app>
    <router-view />
  </v-app>
</template>
