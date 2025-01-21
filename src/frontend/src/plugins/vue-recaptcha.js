import { VueRecaptchaPlugin } from "vue-recaptcha";

const v2SiteKey = import.meta.env.VITE_RECAPTCHA_SITE_KEY_V2;
const v3SiteKey = import.meta.env.VITE_RECAPTCHA_SITE_KEY_V3;

export const recaptcha = VueRecaptchaPlugin;

export const recaptchaConfig = {
  v2SiteKey,
  v3SiteKey
};

export default {
  install: (app) => {
    app.use(VueRecaptchaPlugin, recaptchaConfig);
  }
};
