import { registerPlugins } from "@core/utils/plugins";

// Styles
import "@/assets/styles/admin/template/index.scss";
import "@layouts/styles/index.scss";
import "@/assets/styles/admin/styles.scss";

import { createApp } from "vue";
import { createPinia } from "pinia";
import { useAuthStore } from "./stores/auth";

import App from "./App.vue";

// Create vue app
const app = createApp(App);

// Register plugins
registerPlugins(app);

const pinia = createPinia();

app.use(pinia);

const authStore = useAuthStore();
authStore.initializeStore().then(() => {
  app.mount("#app");
});
