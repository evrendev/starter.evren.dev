// src/plugins/vuetify.ts
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import '@mdi/font/css/materialdesignicons.css' // Bu satırı kontrol edin

export default createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'light'
  },
  icons: {
    defaultSet: 'mdi' // Bu ayarı kontrol edin
  }
})
