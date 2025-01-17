import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import { router } from './router';
import vuetify from './plugins/vuetify';
import '@/scss/style.scss';
import { PerfectScrollbarPlugin } from 'vue3-perfect-scrollbar';
import VueApexCharts from 'vue3-apexcharts';
import VueTablerIcons from 'vue-tabler-icons';
import { i18n } from './plugins/i18n';
import { VueRecaptchaPlugin } from 'vue-recaptcha';
import { createHead } from '@unhead/vue';

import { fakeBackend } from '@/utils/helpers/fake-backend';

// print
import print from 'vue3-print-nb';

const v2SiteKey = import.meta.env.VITE_RECAPTCHA_SITE_KEY_V2;
const v3SiteKey = import.meta.env.VITE_RECAPTCHA_SITE_KEY_V3;

const app = createApp(App);
fakeBackend();
app.use(router);
app.use(PerfectScrollbarPlugin);
app.use(createPinia());
app.use(VueTablerIcons);
app.use(print);
app.use(VueApexCharts);
app.use(vuetify);
app.use(createHead());
app.use(VueRecaptchaPlugin, {
  v2SiteKey,
  v3SiteKey
});
app.use(i18n).mount('#app');
