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
      },
      profile: {
        settings: "Settings",
        logout: "Logout",
        goodMorning: "Good Morning",
        goodAfternoon: "Good Afternoon",
        goodEvening: "Good Evening"
      }
    },
    footerPanel: {
      home: "Home",
      documentation: "Documentation",
      support: "Support"
    },
    sidebar: {
      dashboard: {
        header: "Dashboard",
        title: "Home"
      },
      todos: {
        header: "Todos",
        title: "List"
      },
      admin: {
        title: "Admin"
      },
      users: {
        title: "Users",
        list: "List",
        new: "New"
      },
      roles: {
        title: "Roles",
        list: "List",
        new: "New"
      },
      tenants: {
        title: "Tenants",
        list: "List",
        new: "New"
      },
      audits: {
        title: "Audits"
      }
    }
  },
  messages: {
    success: "Operation successful",
    error: "Request failed"
  },
  admin: {
    starter: {
      title: "Starter Page",
      subtitle: "This is a good place to start"
    },
    audits: {
      title: "Audit Logs",
      list: "List",
      fields: {
        id: "ID",
        user: "User",
        dateTime: "Date Time",
        action: "Action",
        entityType: "Entity Type",
        detail: "Detail"
      },
      actions: {
        insert: "Insert",
        update: "Update",
        delete: "Delete",
        recovered: "Recovered"
      },
      details: {
        title: "Audit Detail"
      }
    },
    tenants: {
      title: "Tenants",
      list: "List",
      fields: {
        id: "ID",
        name: "Name",
        admin: "Admin",
        validUntil: "Valid Until",
        isActive: "Is Active",
        detail: "Detail"
      },
      isActive: {
        true: "Active",
        false: "Passive"
      }
    }
  },
  common: {
    search: "Search",
    filters: "Filters",
    action: "Actions",
    startDate: "Start Date",
    endDate: "End Date",
    selectDate: "Select Date",
    all: "All",
    submit: "Submit",
    reset: "Reset",
    close: "Close"
  },
  $vuetify: {
    ...en
  }
};
