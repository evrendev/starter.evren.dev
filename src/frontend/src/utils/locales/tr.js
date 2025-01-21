import { tr } from "vuetify/locale";

export default {
  auth: {
    login: {
      welcome: "Merhaba, Tekrar Hoşgeldiniz!",
      subtitle: "Giriş yapmak için e-posta ve şifrenizi girin",
      email: {
        label: "E-posta",
        placeholder: "E-posta adresinizi girin",
        invalid: "Geçersiz e-posta adresi",
        required: "E-posta adresi girmeniz gerekmektedir"
      },
      password: {
        label: "Şifre",
        placeholder: "Şifrenizi girin",
        required: "Şifrenizi girmeniz gerekmektedir"
      },
      rememberMe: "Beni Hatırla",
      forgotPassword: "Şifrenizi mi unuttunuz?",
      submit: "Giriş Yap",
      resetForm: "Sıfırla"
    },
    TwoFactorAuthentication: {
      subtitle: "Authenticator uygulamasından kodu girin",
      label: "Kod",
      placeholder: "Kodunuzu girin",
      required: "Kodunuzu girmeniz gerekmektedir",
      submit: "Doğrula"
    },
    forgotPassword: {
      welcome: "Şifrenizi mi unuttunuz?",
      subtitle: "Şifrenizi sıfırlamak için e-postanızı girin",
      email: {
        label: "E-posta",
        placeholder: "E-posta adresinizi girin",
        invalid: "Geçersiz e-posta adresi",
        required: "E-posta adresi girmeniz gerekmektedir"
      },
      back: "Girişe Geri Dön",
      submit: "Şifreyi Sıfırla"
    }
  },
  components: {
    verticalHeader: {
      languages: {
        en: "English",
        tr: "Turkish",
        de: "German"
      }
    },
    footerPanel: {
      home: "Anasayfa",
      documentation: "Dökümantasyon",
      support: "Destek"
    }
  },
  $vuetify: {
    ...tr
  }
};
