import config from "@/config";
import { i18n } from "@/plugins";
import { nextTick } from "vue";

const LocaleHelper = {
  get defaultLocale() {
    return localStorage.getItem("lang") || navigator.language.slice(0, 2) || config.defaultLocale;
  },

  get supportedLocales() {
    return config.languages;
  },

  get currentLocale() {
    return i18n.global.locale.value;
  },

  set currentLocale(newLocale) {
    i18n.global.locale.value = newLocale;
  },

  async switchLanguage(newLocale) {
    await LocaleHelper.loadLocaleMessages(newLocale);
    LocaleHelper.currentLocale = newLocale;
    document.querySelector("html").setAttribute("lang", newLocale);
    document.querySelector("meta").setAttribute("lang", newLocale);
    document.querySelector("meta[http-equiv='content-language']").setAttribute("content", newLocale);

    localStorage.setItem("lang", newLocale);
  },

  async loadLocaleMessages(locale) {
    if (!i18n.global.availableLocales.includes(locale)) {
      const messages = await import(/* webpackChunkName: "locale-[request]" */ `../locales/${locale}.js`);
      i18n.global.setLocaleMessage(locale, messages.default);
    }

    return nextTick();
  },

  isLocaleSupported(locale) {
    return LocaleHelper.supportedLocales.includes(locale);
  },

  getUserLocale() {
    const locale = window.navigator.language || window.navigator.userLanguage || LocaleHelper.defaultLocale;

    return {
      locale: locale,
      localeNoRegion: locale.split("-")[0]
    };
  },

  getPersistedLocale() {
    const persistedLocale = localStorage.getItem("lang");

    if (LocaleHelper.isLocaleSupported(persistedLocale)) {
      return persistedLocale;
    } else {
      return null;
    }
  },

  guessDefaultLocale() {
    const userPersistedLocale = LocaleHelper.getPersistedLocale();
    if (userPersistedLocale) {
      return userPersistedLocale;
    }

    const userPreferredLocale = LocaleHelper.getUserLocale();

    if (LocaleHelper.isLocaleSupported(userPreferredLocale.locale)) {
      return userPreferredLocale.locale;
    }

    if (LocaleHelper.isLocaleSupported(userPreferredLocale.localeNoRegion)) {
      return userPreferredLocale.localeNoRegion;
    }

    return LocaleHelper.defaultLocale;
  },

  async routeMiddleware(to, _from, next) {
    const paramLocale = to.params.locale;

    if (!LocaleHelper.isLocaleSupported(paramLocale)) {
      return next(LocaleHelper.guessDefaultLocale());
    }

    await LocaleHelper.switchLanguage(paramLocale);

    return next();
  }
};

export default LocaleHelper;
