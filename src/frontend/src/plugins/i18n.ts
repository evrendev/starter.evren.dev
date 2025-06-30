import { createI18n } from 'vue-i18n'
import en from '../locales/en.json'
import tr from '../locales/tr.json'

const i18n = createI18n({
  legacy: false, // Composition API kullanmak için
  locale: localStorage.getItem('language') || 'en', // Varsayılan veya depolanmış dil
  fallbackLocale: 'en', // Desteklenmeyen diller için geri dönüş
  messages: {
    en,
    tr
  }
})

export default i18n
