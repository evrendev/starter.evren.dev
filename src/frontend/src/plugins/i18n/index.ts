import config from '@/config';
import { createI18n } from 'vue-i18n';
import en from './locales/en.json';
import de from './locales/de.json';
import tr from './locales/tr.json';

const locale = localStorage.getItem('lang') || navigator.language.slice(0, 2) || import.meta.env.VITE_DEFAULT_LOCALE;

export const i18n = createI18n({
  legacy: false,
  fallbackLocale: config.defaultLocale,
  messages: { en, tr, de },
  locale,
  globalInjection: true,
  runtimeOnly: false
});
