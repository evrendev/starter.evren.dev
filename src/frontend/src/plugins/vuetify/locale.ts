import { useI18n } from "vue-i18n";
import { i18n } from "@/plugins/i18n";
import { createVueI18nAdapter } from "vuetify/locale/adapters/vue-i18n";

export const locale = {
  adapter: createVueI18nAdapter({ i18n, useI18n }),
};

export default locale;
