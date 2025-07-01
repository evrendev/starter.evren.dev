import { createI18n } from 'vue-i18n'
import en from '../locales/en.json'
import de from '../locales/de.json'
import tr from '../locales/tr.json'

const DEFAULT_LANGUAGE = import.meta.env.VITE_APP_DEFAULT_LANGUAGE as string
const FALLBACK_LANGUAGE = import.meta.env.VITE_APP_FALLBACK_LANGUAGE as string

const i18n = createI18n({
  legacy: false,
  locale: localStorage.getItem('language') || DEFAULT_LANGUAGE,
  fallbackLocale: FALLBACK_LANGUAGE,
  messages: {
    en,
    de,
    tr,
  },
})

export default i18n
