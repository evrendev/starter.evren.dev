<script lang="ts">
import { defineComponent } from 'vue'
import { useAuthStore } from '../stores/auth'

export default defineComponent({
  name: 'DashboardView',
  setup() {
    const authStore = useAuthStore()
    const handleLogout = () => {
      authStore.logout()
    }

    return {
      handleLogout
    }
  }
})
</script>

<template>
  <v-container>
    <h1 class="text-h4">
      {{ $t('dashboard') }}
    </h1>
    <p>{{ $t('welcome') }}</p>
    <v-btn color="error" @click="handleLogout">
      {{ $t('logout') }}
    </v-btn>
  </v-container>
</template>
