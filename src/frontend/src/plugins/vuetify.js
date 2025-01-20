import { createVuetify } from "vuetify";
import { aliases, mdi } from "vuetify/iconsets/mdi-svg";
import { icons } from "./mdi-icon";
import { NavyTheme } from "@/theme/LightTheme";
import { createVueI18nAdapter } from "vuetify/locale/adapters/vue-i18n";
import { createI18n } from "./i18n";
import { useI18n } from "vue-i18n";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
// Fix export syntax
const vuetify = createVuetify({
  components,
  directives,
  icons: {
    defaultSet: "mdi",
    aliases: {
      ...aliases,
      ...icons
    },
    sets: {
      mdi
    }
  },
  theme: {
    defaultTheme: "NavyTheme",
    themes: {
      NavyTheme
    }
  },
  locale: {
    adapter: createVueI18nAdapter({ createI18n, useI18n })
  },
  defaults: {
    VBtn: {},
    VCard: {
      rounded: "md"
    },
    VTextField: {
      rounded: "lg"
    },
    VTooltip: {
      location: "top"
    }
  }
});

export default vuetify;
