<script lang="ts">
import { defineComponent, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

export default defineComponent({
  name: 'LoginView',
  setup() {
    const email = ref('')
    const password = ref('')
    const router = useRouter()
    const authStore = useAuthStore()

    const handleLogin = async () => {
      try {
        await authStore.login(email.value, password.value)
        router.push({ name: 'dashboard' })
      } catch (error) {
        console.error('Login failed:', error)
      }
    }

    return {
      email,
      password,
      handleLogin
    }
  }
})
</script>

<template>
  <v-container class="fill-height" fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="4">
        <v-card class="elevation-12">
          <v-toolbar color="primary" dark flat>
            <v-toolbar-title>{{ $t('login') }}</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form @submit.prevent="handleLogin">
              <v-text-field
                v-model="email"
                :label="$t('username')"
                prepend-inner-icon="mdi-account"
                variant="outlined"
                density="compact"
                required
              ></v-text-field>
              <v-text-field
                v-model="password"
                :label="$t('password')"
                prepend-inner-icon="mdi-lock"
                variant="outlined"
                density="compact"
                type="password"
                required
              ></v-text-field>
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary" @click="handleLogin">{{ $t('submit') }}</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
