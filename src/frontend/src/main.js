import "@/scss/style.scss"
import App from "./App.vue"
import { createApp } from "vue"
import { createPinia } from "pinia"
import { router } from "./router"
import { PerfectScrollbarPlugin } from "vue3-perfect-scrollbar"
// import VueApexCharts from 'vue3-apexcharts';
// import VueTablerIcons from 'vue-tabler-icons';
import { VueRecaptchaPlugin } from "vue-recaptcha"
import { createHead } from "@unhead/vue"

// print
// import print from 'vue3-print-nb';

import { createI18n, useI18n } from "vue-i18n"
import { tr, en, de } from "@/utils/locales"

const locale =
  localStorage.getItem("lang") ||
  navigator.language.slice(0, 2) ||
  import.meta.env.VITE_DEFAULT_LOCALE

const i18n = createI18n({
  locale,
  fallbackLocale: import.meta.env.VITE_FALLBACK_LOCALE,
  legacy: false,
  globalInjection: true,
  messages: { tr, en, de },
  runtimeOnly: false,
})

import { createVuetify } from "vuetify"
import { aliases, mdi } from "vuetify/iconsets/mdi-svg"
import { icons } from "@/plugins/mdi-icon"
import { NavyTheme } from "@/theme/LightTheme"
import { createVueI18nAdapter } from "vuetify/locale/adapters/vue-i18n"
import * as components from "vuetify/components"
import * as directives from "vuetify/directives"
// Fix export syntax
const vuetify = createVuetify({
  components,
  directives,
  icons: {
    defaultSet: "mdi",
    aliases: {
      ...aliases,
      ...icons,
    },
    sets: {
      mdi,
    },
  },
  theme: {
    defaultTheme: "NavyTheme",
    themes: {
      NavyTheme,
    },
  },
  locale: {
    adapter: createVueI18nAdapter({ i18n, useI18n }),
  },
  defaults: {
    VBtn: {},
    VCard: {
      rounded: "md",
    },
    VTextField: {
      rounded: "lg",
    },
    VTooltip: {
      location: "top",
    },
  },
})

const v2SiteKey = import.meta.env.VITE_RECAPTCHA_SITE_KEY_V2
const v3SiteKey = import.meta.env.VITE_RECAPTCHA_SITE_KEY_V3

const app = createApp(App)

app.use(router)
app.use(PerfectScrollbarPlugin)
app.use(createPinia())
// app.use(VueTablerIcons);
// app.use(print);
// app.use(VueApexCharts);
app.use(i18n)
app.use(vuetify)
app.use(createHead())
app.use(VueRecaptchaPlugin, {
  v2SiteKey,
  v3SiteKey,
})
app.mount("#app")
