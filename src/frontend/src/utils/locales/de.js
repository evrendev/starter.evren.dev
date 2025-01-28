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
      recaptcha: {
        label: "Ich bin kein Roboter",
        required: "Recaptcha ist erforderlich",
        invalid: "Recaptcha-Überprüfung fehlgeschlagen"
      },
      rememberMe: "Erinnere dich an mich",
      forgotPassword: "Passwort vergessen?",
      submit: "Anmelden",
      resetForm: "Zurücksetzen"
    },
    TwoFactorAuthentication: {
      subtitle: "Geben Sie den Code aus der Authenticator-App ein",
      label: "Code",
      placeholder: "Geben Sie Ihren Code ein",
      required: "Code ist erforderlich",
      submit: "Überprüfen"
    },
    forgotPassword: {
      welcome: "Passwort vergessen?",
      subtitle: "Geben Sie Ihre E-Mail-Adresse ein, um Ihr Passwort zurückzusetzen",
      email: {
        label: "E-Mail",
        placeholder: "Geben Sie Ihre E-Mail-Adresse ein",
        invalid: "Ungültige E-Mail-Adresse",
        required: "E-Mail ist erforderlich"
      },
      back: "Zurück zur Anmeldung",
      submit: "Passwort zurücksetzen"
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
        settings: "Einstellungen",
        logout: "Abmelden",
        goodMorning: "Guten Morgen",
        goodAfternoon: "Guten Tag",
        goodEvening: "Guten Abend"
      }
    },
    footerPanel: {
      home: "Startseite",
      documentation: "Dokumentation",
      support: "Unterstützung"
    },
    sidebar: {
      dashboard: {
        header: "Dashboard",
        title: "Startseite"
      },
      todos: {
        header: "Todos",
        title: "Listen"
      },
      admin: {
        title: "Admin"
      },
      users: {
        title: "Benutzer",
        list: "Liste",
        new: "Neu"
      },
      roles: {
        title: "Rollen",
        list: "Liste",
        new: "Neu"
      },
      tenants: {
        title: "Mandanten",
        list: "Liste",
        new: "Neu"
      },
      audits: {
        title: "Audits"
      }
    }
  },
  messages: {
    success: "Vorgang erfolgreich",
    error: "Anfrage fehlgeschlagen"
  },
  admin: {
    starter: {
      title: "Startseite",
      subtitle: "Willkommen bei der Admin-Panel"
    },
    audits: {
      title: "Audits",
      list: "Liste",
      fields: {
        id: "ID",
        user: "Benutzer",
        dateTime: "Datum/Uhrzeit",
        action: "Aktion",
        entityType: "Entitätstyp",
        detail: "Detail"
      },
      actions: {
        insert: "Einfügen",
        update: "Aktualisieren",
        delete: "Löschen",
        recovered: "Wiederhergestellt"
      },
      details: {
        title: "Audit-Detail"
      }
    },
    tenants: {
      title: "Mandanten",
      list: "Liste",
      fields: {
        id: "ID",
        name: "Name",
        admin: "Admin",
        validUntil: "Gültig bis",
        isActive: "Ist aktiv",
        process: "Prozess"
      },
      isActive: {
        true: "Active",
        false: "Passive"
      }
    }
  },
  common: {
    search: "Suche",
    filters: "Filter",
    action: "Aktion",
    startDate: "Startdatum",
    endDate: "Enddatum",
    selectDate: "Datum auswählen",
    all: "Alle",
    submit: "Einreichen",
    reset: "Zurücksetzen",
    close: "Schließen"
  },
  $vuetify: {
    ...de
  }
};
