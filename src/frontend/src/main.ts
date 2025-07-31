import { registerPlugins } from "@core/utils/plugins";

// Styles
import "@core/scss/template/index.scss";
import "@layouts/styles/index.scss";
import "@styles/styles.scss";

import { createApp } from "vue";
import { createPinia } from "pinia";

import App from "./App.vue";

// Create vue app
const app = createApp(App);

// Register plugins
registerPlugins(app);

const pinia = createPinia();

app.use(pinia);

app.mount("#app");
