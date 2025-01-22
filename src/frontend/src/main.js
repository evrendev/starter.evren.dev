import "@/scss/style.scss";
import App from "./App.vue";
import { createApp } from "vue";
import { router } from "./router";
import { createHead } from "@unhead/vue";
import { pinia, i18n, vuetify, perfectScrollbarPlugin, recaptchaPlugin } from "@/plugins";

const app = createApp(App);

app.use(router);
app.use(perfectScrollbarPlugin);
app.use(pinia);
app.use(i18n);
app.use(vuetify);
app.use(recaptchaPlugin);
app.use(createHead());
app.mount("#app");
