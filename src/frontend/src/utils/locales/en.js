import { en } from "vuetify/locale";

export default {
  auth: {
    login: {
      welcome: "Hi, Welcome Back!",
      subtitle: "Enter your email and password to sign in",
      email: {
        label: "Email",
        placeholder: "Enter your email",
        invalid: "Invalid email address",
        required: "Email is required"
      },
      password: {
        label: "Password",
        placeholder: " Enter your password",
        required: "Password is required"
      },
      recaptcha: {
        label: "I'm not a robot",
        required: "Recaptcha is required",
        invalid: "Recaptcha verification failed"
      },
      rememberMe: "Remember Me",
      forgotPassword: "Forgot Password?",
      submit: "Sign In",
      resetForm: "Reset"
    },
    TwoFactorAuthentication: {
      subtitle: "Enter the code from authenticator app",
      label: "Code",
      placeholder: "Enter your code",
      required: "Code is required",
      submit: "Verify"
    },
    forgotPassword: {
      welcome: "Forgot Password?",
      subtitle: "Enter your email to reset your password",
      email: {
        label: "Email",
        placeholder: "Enter your email",
        invalid: "Invalid email address",
        required: "Email is required"
      },
      back: "Back to Sign In",
      submit: "Reset Password"
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
      home: "Home",
      documentation: "Documentation",
      support: "Support"
    }
  },
  $vuetify: {
    ...en
  }
};
