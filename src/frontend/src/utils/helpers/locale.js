import config from "@/config";
import { i18n } from "@/plugins";
import { nextTick } from "vue";

class LocaleHelper {
  static #STORAGE_KEY = "lang";

  static get defaultLocale() {
    return localStorage.getItem(this.#STORAGE_KEY) || navigator.language.slice(0, 2) || config.defaultLocale;
  }

  static get supportedLocales() {
    return config.languages;
  }

  static get currentLocale() {
    return i18n.global.locale.value;
  }

  static set currentLocale(newLocale) {
    i18n.global.locale.value = newLocale;
  }

  static async switchLanguage(newLocale) {
    if (!this.isLocaleSupported(newLocale)) {
      throw new Error(`Locale ${newLocale} is not supported`);
    }

    await this.loadLocaleMessages(newLocale);
    this.currentLocale = newLocale;
    this.updateDocumentLang(newLocale);
    this.persistLocale(newLocale);
  }

  static async loadLocaleMessages(locale) {
    try {
      if (!i18n.global.availableLocales.includes(locale)) {
        const messages = await import(`../locales/${locale}.js`);
        i18n.global.setLocaleMessage(locale, messages.default);
      }
      return nextTick();
    } catch (error) {
      console.error(`Failed to load locale messages for ${locale}:`, error);
      throw error;
    }
  }

  static isLocaleSupported(locale) {
    return this.supportedLocales.includes(locale);
  }

  static getUserLocale() {
    const locale = window.navigator.language || window.navigator.userLanguage || this.defaultLocale;

    return {
      locale,
      localeNoRegion: locale.split("-")[0]
    };
  }

  static getPersistedLocale() {
    const persistedLocale = localStorage.getItem(this.#STORAGE_KEY);
    return this.isLocaleSupported(persistedLocale) ? persistedLocale : null;
  }

  static persistLocale(locale) {
    localStorage.setItem(this.#STORAGE_KEY, locale);
  }

  static updateDocumentLang(locale) {
    document.documentElement.setAttribute("lang", locale);
    document.querySelector("meta[name='language']")?.setAttribute("content", locale);
    document.querySelector("meta[http-equiv='content-language']")?.setAttribute("content", locale);
  }

  static guessDefaultLocale() {
    const userPersistedLocale = this.getPersistedLocale();
    if (userPersistedLocale) {
      return userPersistedLocale;
    }

    const { locale, localeNoRegion } = this.getUserLocale();

    if (this.isLocaleSupported(locale)) {
      return locale;
    }

    if (this.isLocaleSupported(localeNoRegion)) {
      return localeNoRegion;
    }

    return this.defaultLocale;
  }

  static async routeMiddleware(to, _from, next) {
    const paramLocale = to.params.locale;

    if (!this.isLocaleSupported(paramLocale)) {
      return next(this.guessDefaultLocale());
    }

    await this.switchLanguage(paramLocale);
    return next();
  }
}

export default LocaleHelper;
