import { createI18n } from "vue-i18n";
import { tr, en, de } from "@/utils/locales";

const locale = localStorage.getItem("lang") || navigator.language.slice(0, 2) || import.meta.env.VITE_DEFAULT_LOCALE;

export default createI18n({
  locale,
  fallbackLocale: import.meta.env.VITE_FALLBACK_LOCALE,
  legacy: false,
  globalInjection: true,
  messages: { tr, en, de },
  runtimeOnly: false
});
