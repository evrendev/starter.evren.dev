<script setup lang="ts">
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'
import type { Result } from '@/primitives/result'
import { useRouter } from 'vue-router'
import type { AccessTokenResponse } from '@/responses/auth'

const authStore = useAuthStore()
const router = useRouter()

const email = ref<string>('')
const password = ref<string>('')

async function login(): Promise<void> {
  const result: Result<AccessTokenResponse> = await authStore.login(email.value, password.value)

  if (result.succeeded) {
    router.replace({ name: 'home' })
  }
}
</script>

<template>
  <section id="login">
    <form @submit.prevent="login">
      <h2>Login</h2>
      <div class="form-group mb-3">
        <label class="form-label" for="email">Email</label>
        <input id="email" v-model="email" class="form-control" type="email" />
      </div>
      <div class="form-group mb-3">
        <label class="form-label" for="password">Password</label>
        <input id="password" v-model="password" class="form-control" type="password" />
      </div>
      <button class="btn btn-primary" type="submit">Login</button>
    </form>
  </section>
</template>
