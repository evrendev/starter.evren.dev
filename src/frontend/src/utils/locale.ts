// @/utils/locale.ts

export interface Language {
  code: string;
  name: string;
  flag?: string;
}

// Mevcut diller
export const AVAILABLE_LANGUAGES: Language[] = [
  { code: "en", name: "English", flag: "🇺🇸" },
  { code: "de", name: "Deutsch", flag: "🇩🇪" },
  { code: "tr", name: "Türkçe", flag: "🇹🇷" },
];

// Mevcut dillerin listesini al
export const getAvailableLanguages = (): Language[] => {
  return AVAILABLE_LANGUAGES;
};

// localStorage'dan veya tarayıcı tercihinden kaydedilen dili al
export const getSavedLanguage = (): string => {
  const saved = localStorage.getItem("locale");
  if (saved && AVAILABLE_LANGUAGES.some((lang) => lang.code === saved)) {
    return saved;
  }

  // Tarayıcı diline geri dön
  const browserLang = navigator.language.split("-")[0];
  return AVAILABLE_LANGUAGES.some((lang) => lang.code === browserLang)
    ? browserLang
    : import.meta.env.VITE_APP_DEFAULT_LANGUAGE;
};
