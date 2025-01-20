import { de } from "vuetify/locale";

export default {
  auth: {
    login: {
      welcome: "Hallo, Willkommen zurück!",
      subtitle: "Geben Sie Ihre E-Mail-Adresse und Ihr Passwort ein, um sich anzumelden",
      email: {
        label: "E-Mail",
        placeholder: "Geben Sie Ihre E-Mail-Adresse ein",
        invalid: "Ungültige E-Mail-Adresse",
        required: "E-Mail ist erforderlich"
      },
      password: {
        label: "Passwort",
        placeholder: "Geben Sie Ihr Passwort ein",
        required: "Passwort ist erforderlich"
      },
      code: {
        subtitle: "Geben Sie den Code aus der Authenticator-App ein",
        label: "Code",
        placeholder: "Geben Sie Ihren Code ein",
        required: "Code ist erforderlich"
      },
      rememberMe: "Erinnere dich an mich",
      forgotPassword: "Passwort vergessen?",
      submit: "Anmelden",
      resetForm: "Zurücksetzen"
    },
    required: {
      email: "E-Mail ist erforderlich",
      password: "Passwort ist erforderlich",
      passwordLength: "Das Passwort muss mindestens 6 Zeichen lang sein"
    }
  },
  $vuetify: {
    ...de
  }
};
