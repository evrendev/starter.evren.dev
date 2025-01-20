import { en } from "vuetify/locale"

export default {
  auth: {
    login: {
      welcome: "Hi, Welcome Back!",
      subtitle: "Enter your email and password to sign in",
      email: {
        label: "Email",
        placeholder: "Enter your email",
        invalid: "Invalid email address",
        required: "Email is required",
      },
      password: {
        label: "Password",
        placeholder: " Enter your password",
        required: "Password is required",
      },
      rememberMe: "Remember Me",
      forgotPassword: "Forgot Password?",
      submit: "Sign In",
    },
    required: {
      email: "Email is required",
      password: "Password is required",
      passwordLength: "Password must be at least 6 characters",
    },
  },
  $vuetify: {
    ...en,
  },
}
