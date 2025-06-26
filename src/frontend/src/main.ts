import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import { createPinia } from "pinia";

// Plugins (daha modüler bir yaklaşım için)
import i18n from "./plugins/i18n";
import vuetify from "./plugins/vuetify";

// Global stiller
import "./assets/styles/main.scss";

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(vuetify);
app.use(i18n);

app.mount("#app");
