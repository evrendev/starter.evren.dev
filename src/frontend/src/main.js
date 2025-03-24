import "@/scss/style.scss";
import App from "./App.vue";
import { createApp } from "vue";
import { router } from "./router";
import { pinia, i18n, vuetify, perfectScrollbarPlugin } from "@/plugins";

const app = createApp(App);

app.use(router);
app.use(perfectScrollbarPlugin);
app.use(pinia);
app.use(i18n);
app.use(vuetify);
app.mount("#app");
