import config from '@/config';
import { createI18n } from 'vue-i18n';

const messages = {
  en: {
    auth: {
      login: {
        welcomeText: 'Hi, Welcome Back!',
        subTitle: 'Enter your email and password to sign in'
      },
      required: {
        email: 'Email is required',
        password: 'Password is required',
        passwordLength: 'Password must be at least 6 characters'
      }
    }
  },
  tr: {
    auth: {
      login: {
        welcomeText: 'Merhaba, Tekrar Hoşgeldiniz!',
        subTitle: 'Giriş yapmak için e-posta ve şifrenizi girin'
      },
      required: {
        email: 'E-posta adresi girilmesi zorunludur',
        password: 'Şifre girilmesi zorunludur'
      }
    }
  },
  de: {
    auth: {
      login: {
        welcomeText: 'Hallo, Willkommen zurück!',
        subTitle: 'Geben Sie Ihre E-Mail und Ihr Passwort ein, um sich anzumelden'
      },
      required: {
        email: 'E-Mail ist erforderlich',
        password: 'Passwort ist erforderlich'
      }
    }
  }
};

export const i18n = createI18n({
  legacy: false,
  locale: config.defaultLocale,
  fallbackLocale: config.defaultLocale,
  messages
});
