import { en } from "vuetify/locale";

export default {
  auth: {
    login: {
      title: "Sign In",
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
    twoFactorAuth: {
      subtitle: "Enter the code from authenticator app",
      label: "Code",
      placeholder: "Enter your code",
      required: "Code is required",
      submit: "Verify",
      code: {
        required: "Code is required",
        invalid: "Invalid code"
      }
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
        title: "My Profile",
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
        title: "List Todos"
      },
      donations: {
        title: "Donations",
        fontains: {
          title: "Fountain Donations",
          all: "All Donations",
          bks: "BKS",
          bgs: "BGS",
          aki: "AKI",
          agi: "AGI"
        },
        new: "New Donation"
      },
      admin: {
        title: "Admin"
      },
      users: {
        title: "Users",
        list: "List Users",
        new: "Add User"
      },
      roles: {
        title: "Roles",
        list: "List Roles",
        new: "Add Role"
      },
      tenants: {
        title: "Tenants",
        list: "List Tenants",
        new: "Add Tenant"
      },
      audits: {
        title: "Audit Logs"
      }
    }
  },
  messages: {
    success: "Operation successful",
    error: "Request failed"
  },
  admin: {
    dashboard: {
      title: "Dashboard",
      subtitle: "This is a good place to start"
    },
    profile: {
      title: "Profile",
      email: "Email",
      firstName: "First Name",
      lastName: "Last Name",
      gender: "Gender",
      jobTitle: "Job Title",
      language: "Language",
      validation: {
        firstName: {
          required: "First name is required",
          maxLength: "First name must be less than {max} characters"
        },
        lastName: {
          required: "Last name is required",
          maxLength: "Last name must be less than {max} characters"
        },
        jobTitle: {
          maxLength: "Job title must be less than {max} characters"
        },
        email: {
          required: "Email is required",
          invalid: "Invalid email address"
        },
        language: {
          required: "Language is required"
        },
        gender: {
          required: "Gender is required"
        },
        twoFactorAuth: {
          code: {
            required: "Code is required",
            invalid: "Invalid code"
          }
        }
      },
      twoFactorAuth: {
        title: "Two-Factor Authentication",
        description: "Add an extra layer of security to your account by enabling two-factor authentication.",
        enable: "Enable 2FA",
        disable: "Disable 2FA",
        confirmDisable: {
          title: "Disable Two-Factor Authentication",
          message: "Are you sure you want to disable two-factor authentication? This will make your account less secure."
        },
        setup: {
          title: "Set up Two-Factor Authentication",
          intro: "To set up two-factor authentication, scan the QR code below with your authenticator app or enter the code manually.",
          start: "Get Started",
          manual: "Or enter this code manually in your authenticator app:",
          code: "Enter verification code"
        }
      }
    },
    audits: {
      title: "Audit Logs",
      list: "List Audit Logs",
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
    donations: {
      title: "Donations",
      list: "List Donations",
      fields: {
        id: "ID",
        contact: "Contact",
        phone: "Phone",
        creationDate: "Date",
        info: "Info",
        projectCode: "Project Code",
        weeks: "Weeks",
        status: "Status",
        team: "Team",
        detail: "Action"
      },
      projectCodes: {
        bks: "BKS",
        bgs: "BGS",
        aki: "AKI",
        agi: "AGI"
      },
      status: {
        none: "No details available",
        initialWeek: "Initial Week",
        ongoingEarlyWeeks: "Ongoing Early Weeks",
        week5Media: "Week 5 Media",
        week6Warning: "Week 6 Warning",
        week8Critical: "Week 8 Critical",
        published: "Published"
      },
      details: {
        creationDate: "Date",
        weeks: "Weeks",
        team: "Team",
        contact: "Contact",
        phone: "Phone",
        project: "Project",
        projectCode: "Project Code",
        projectNumber: "Project Number",
        banner: "Banner",
        transactionId: "Transaction ID",
        source: "Source",
        link: "Link"
      }
    },
    tenants: {
      title: "Tenants",
      list: "List Tenants",
      new: "New Tenant",
      edit: "Edit Tenant",
      fields: {
        id: "ID",
        name: "Name",
        adminEmail: "Email",
        validUntil: "Valid Until",
        isActive: "Is Active",
        process: "Process",
        description: "Description",
        connectionString: "Connection String",
        host: "Host"
      },
      delete: {
        title: "Delete Tenant",
        message: "Are you sure you want to delete this tenant?"
      },
      restore: {
        title: "Restore Tenant",
        message: "Are you sure you want to restore this tenant?"
      },
      activate: {
        title: "Activate Tenant",
        message: "Are you sure you want to activate this tenant?"
      },
      deactivate: {
        title: "Deactivate Tenant",
        message: "Are you sure you want to deactivate this tenant?"
      },
      validation: {
        name: {
          required: "Tenant name is required",
          maxLength: "Tenant name must be less than {max} characters"
        },
        description: {
          maxLength: "Description must be less than {max} characters"
        },
        adminEmail: {
          required: "Admin email is required",
          invalid: "Invalid email address"
        },
        validUntil: {
          required: "Valid until date is required",
          invalid: "Invalid date",
          future: "You must select a future date"
        }
      },
      helpers: {
        information: "Tenant information",
        connection: "Connection information"
      }
    },
    roles: {
      title: "Roles",
      list: "Roles List",
      new: "New Role",
      edit: "Edit Role",
      fields: {
        id: "ID",
        name: "Name",
        description: "Description",
        process: "Process"
      },
      delete: {
        title: "Delete Role",
        message: "Are you sure you want to delete this role?"
      },
      validation: {
        name: {
          required: "Role name is required",
          maxLength: "Role name must be less than {max} characters"
        },
        description: {
          maxLength: "Description must be less than {max} characters"
        }
      }
    },
    users: {
      title: "Users",
      list: "List Users",
      new: "New User",
      edit: "Edit User",
      delete: {
        title: "Delete User",
        message: "Are you sure you want to delete this user?",
        options: {
          true: "Yes",
          false: "No"
        }
      },
      restore: {
        title: "Restore User",
        message: "Are you sure you want to restore this user?"
      },
      fields: {
        initial: "#",
        twoFactorEnabled: "2FA",
        gender: "Gender",
        firstName: "First Name",
        lastName: "Last Name",
        email: "Email",
        jobTitle: "Job Title",
        process: "Process",
        language: "Language",
        password: "Password",
        confirmPassword: "Confirm Password"
      },
      validation: {
        gender: {
          required: "Gender is required"
        },
        email: {
          required: "Email is required",
          invalid: "Invalid email address"
        },
        firstName: {
          required: "First name is required",
          maxLength: "First name must be less than {max} characters"
        },
        lastName: {
          required: "Last name is required",
          maxLength: "Last name must be less than {max} characters"
        },
        password: {
          required: "Password is required",
          minLength: "Password must be at least {min} characters",
          uppercase: "Password must contain at least one uppercase letter",
          lowercase: "Password must contain at least one lowercase letter",
          number: "Password must contain at least one number",
          special: "Password must contain at least one special character"
        },
        confirmPassword: {
          required: "Confirm password is required",
          match: "Passwords do not match"
        },
        jobTitle: {
          maxLength: "Job title must be less than {max} characters"
        },
        permissions: {
          required: "At least one permission is required"
        }
      },
      helpers: {
        information: "User information",
        permissions: "User permissions"
      }
    }
  },
  error: {
    title: "Something went wrong",
    description: "The page you are looking was moved, removed, renamed, or might never exist!",
    home: "Home"
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
    save: "Save",
    reset: "Reset",
    update: "Update",
    edit: "Edit Information",
    close: "Close",
    cancel: "Cancel",
    verify: "Verify",
    confirm: "Confirm",
    deleted: "Successfully deleted",
    selectAll: "Select All",
    error: "Error",
    navigation: "Navigation",
    goToTop: "Go to top",
    showDeletedItems: "Show Deleted Items",
    showActiveItems: "Show Active Items",
    copy: "Copy",
    copied: "Copied",
    showDetails: "Show Details",
    modules: {
      donations: "Donations",
      todos: "Todos",
      tenants: "Tenants",
      roles: "Roles",
      images: "Images",
      users: "Users",
      audits: "Audits",
      files: "Files"
    },
    permissions: {
      read: "Read",
      create: "Create",
      delete: "Delete",
      restore: "Restore",
      edit: "Edit"
    },
    active: "Active",
    passive: "Passive",
    true: "Yes",
    false: "No"
  },
  $vuetify: {
    ...en
  }
};
