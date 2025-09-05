import type { App } from "vue";
import { createI18n } from "vue-i18n";
import { deutsch, english, turkish } from "@/locales";
import { de, en, tr } from "vuetify/locale";

const DEFAULT_LANGUAGE =
  (import.meta.env.VITE_APP_DEFAULT_LANGUAGE as string) || "en";
const FALLBACK_LANGUAGE =
  (import.meta.env.VITE_APP_FALLBACK_LANGUAGE as string) || "en";

const i18n = createI18n({
  legacy: false,
  locale: localStorage.getItem("locale") || DEFAULT_LANGUAGE,
  fallbackLocale: FALLBACK_LANGUAGE,
  messages: {
    en: {
      ...english,
      $vuetify: {
        ...en,
      },
    },
    de: {
      ...deutsch,
      $vuetify: {
        ...de,
      },
    },
    tr: {
      ...turkish,
      $vuetify: {
        ...tr,
      },
    },
  },
});

// Vue plugin function that will be auto-registered
export default function (app: App) {
  app.use(i18n);
}

export { i18n };
