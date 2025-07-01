import './assets/styles/main.scss'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { useAuthStore } from './stores/auth'

import App from './App.vue'

import router from './plugins/router'
import i18n from './plugins/i18n'
import vuetify from './plugins/vuetify'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

const authStore = useAuthStore()
authStore.initializeStore().then(() => {
  app.use(router)
  app.use(vuetify)
  app.use(i18n)
  app.mount('#app')
})
