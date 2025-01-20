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
      code: {
        subtitle: "Authenticator uygulamasından kodu girin",
        label: "Kod",
        placeholder: "Kodunuzu girin",
        required: "Kodunuzu girmeniz gerekmektedir"
      },
      rememberMe: "Beni Hatırla",
      forgotPassword: "Şifrenizi mi unuttunuz?",
      submit: "Giriş Yap",
      resetForm: "Sıfırla"
    },
    required: {
      email: "E-posta adresinizi girmeniz gerekmektedir",
      password: "Şifrenizi girmeniz gerekmektedir",
      passwordLength: "Şifre en az 6 karakter olmalıdır"
    }
  },
  $vuetify: {
    ...tr
  }
};
