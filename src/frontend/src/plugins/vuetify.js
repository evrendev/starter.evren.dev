import { i18n } from "@/plugins";
import { useI18n } from "vue-i18n";

import { createVuetify } from "vuetify";
import { aliases, mdi } from "vuetify/iconsets/mdi-svg";
import { icons } from "@/plugins/mdi-icon";
import { LightTheme, DarkTheme } from "@/theme";
import { createVueI18nAdapter } from "vuetify/locale/adapters/vue-i18n";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
// Fix export syntax
export default createVuetify({
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
    defaultTheme: "LightTheme",
    themes: {
      LightTheme,
      DarkTheme
    }
  },
  locale: {
    adapter: createVueI18nAdapter({ i18n, useI18n })
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
