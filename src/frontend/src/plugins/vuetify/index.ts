import type { App } from "vue";

import { createVuetify } from "vuetify";
import { VBtn } from "vuetify/components/VBtn";
import defaults from "./defaults";
import { icons } from "./icons";
import { themes } from "./theme";
import { locale } from "./locale";
import { date } from "./date";

// Styles
import "vuetify/_styles.scss";

export default function (app: App) {
  const vuetify = createVuetify({
    aliases: {
      IconBtn: VBtn,
    },
    defaults,
    icons,
    locale,
    date,
    theme: {
      defaultTheme: "light",
      themes,
    },
  });

  app.use(vuetify);
}
