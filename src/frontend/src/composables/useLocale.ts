import { computed } from "vue";

export const useAppLocale = () => {
  const { locale, availableLocales, t } = useI18n();

  const currentLocale = computed({
    get: () => locale.value,
    set: (newLocale: string) => {
      locale.value = newLocale;
      localStorage.setItem("language", newLocale);
    },
  });

  const changeLocale = (newLocale: string) => {
    if (availableLocales.includes(newLocale)) {
      currentLocale.value = newLocale;
    }
  };

  const getAvailableLocales = () => availableLocales;

  return {
    currentLocale,
    changeLocale,
    getAvailableLocales,
    t,
  };
};
