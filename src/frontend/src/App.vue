<script setup lang="ts">
const theme = useTheme()
provide(
  'themeMode',
  computed(() => (theme.current.value.dark ? 'dark' : undefined)),
)
</script>

<template>
  <v-app>
    <router-view v-slot="{ Component }">
      <v-fade-transition hide-on-leave>
        <component :is="Component" />
      </v-fade-transition>
    </router-view>
  </v-app>
</template>
