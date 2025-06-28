<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const permissions = authStore.user?.permissions || []
</script>

<template>
  <section id="permissions">
    <h1>Permissions</h1>
    <ul>
      <li v-for="permission in permissions" :key="permission">
        <span>{{ permission }}</span>
      </li>
    </ul>
  </section>
</template>
