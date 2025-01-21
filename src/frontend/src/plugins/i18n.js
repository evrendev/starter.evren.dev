import { createI18n } from "vue-i18n";
import { tr, en, de } from "@/utils/locales";
import config from "@/config";

const locale = localStorage.getItem("lang") || navigator.language.slice(0, 2) || config.defaultLocale;

const i18n = createI18n({
  locale,
  fallbackLocale: config.fallbackLocale,
  legacy: false,
  globalInjection: true,
  messages: { tr, en, de },
  runtimeOnly: false
});

export default i18n;
