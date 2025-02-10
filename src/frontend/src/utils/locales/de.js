import { de } from "vuetify/locale";

export default {
  auth: {
    login: {
      title: "Anmelden",
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
    twoFactorAuth: {
      subtitle: "Geben Sie den Code aus der Authenticator-App ein",
      label: "Code",
      placeholder: "Geben Sie Ihren Code ein",
      required: "Code ist erforderlich",
      submit: "Überprüfen",
      code: {
        required: "Code ist erforderlich",
        invalid: "Ungültiger Code"
      }
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
        title: "Mein Profil",
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
    dashboard: {
      title: "Dashboard",
      subtitle: "Willkommen bei der Admin-Panel"
    },
    profile: {
      title: "Profil",
      email: "E-Mail",
      firstName: "Vorname",
      lastName: "Nachname",
      gender: "Geschlecht",
      jobTitle: "Berufsbezeichnung",
      language: "Sprache",
      validation: {
        firstName: {
          required: "Vorname ist erforderlich",
          maxLength: "Vorname muss weniger als {max} Zeichen lang sein"
        },
        lastName: {
          required: "Nachname ist erforderlich",
          maxLength: "Nachname muss weniger als {max} Zeichen lang sein"
        },
        jobTitle: {
          maxLength: "Berufsbezeichnung muss weniger als {max} Zeichen lang sein"
        },
        email: {
          required: "E-Mail ist erforderlich",
          invalid: "Ungültige E-Mail-Adresse"
        },
        language: {
          required: "Sprache ist erforderlich"
        },
        gender: {
          required: "Geschlecht ist erforderlich"
        },
        twoFactorAuth: {
          code: {
            required: "Code ist erforderlich",
            invalid: "Ungültiger Code"
          }
        }
      },
      twoFactorAuth: {
        title: "Zwei-Faktor-Authentifizierung",
        description: "Zwei-Faktor-Authentifizierung ist deaktiviert. Sie können es aktivieren, um die Sicherheit Ihres Kontos zu erhöhen.",
        enable: "Aktivieren 2FA",
        disable: "Deaktivieren 2FA",
        setup: {
          title: "Richten Sie die Zwei-Faktor-Authentifizierung ein",
          intro:
            "Scannen Sie den QR-Code mit Ihrer Authenticator-App, um die Zwei-Faktor-Authentifizierung zu aktivieren. Wenn Sie keinen QR-Code scannen können, geben Sie den Code manuell ein.",
          start: "Starten Sie die Einrichtung",
          manual: "Oder geben Sie diesen Code manuell in Ihrer Authenticator-App ein:",
          code: "Code"
        }
      }
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
      new: "Neu",
      edit: "Bearbeiten",
      fields: {
        id: "ID",
        name: "Name",
        adminEmail: "E-Mail",
        validUntil: "Gültig bis",
        isActive: "Ist aktiv",
        process: "Prozess",
        description: "Beschreibung",
        connectionString: "Verbindungszeichenfolge",
        host: "Host"
      },
      activeOptions: {
        true: "Active",
        false: "Passive"
      },
      delete: {
        title: "Mandant löschen",
        message: "Möchten Sie den Mandanten löschen?"
      },
      activate: {
        title: "Mandant aktivieren",
        message: "Möchten Sie den Mandanten aktivieren?"
      },
      deactivate: {
        title: "Mandant deaktivieren",
        message: "Möchten Sie den Mandanten deaktivieren?"
      },
      validation: {
        name: {
          required: "Mandantenname ist erforderlich",
          maxLength: "Mandantenname muss weniger als {max} Zeichen lang sein"
        },
        description: {
          maxLength: "Beschreibung muss weniger als {max} Zeichen lang sein"
        },
        adminEmail: {
          required: "Admin-E-Mail ist erforderlich",
          invalid: "Ungültige E-Mail-Adresse"
        },
        validUntil: {
          required: "Gültig bis ist erforderlich",
          invalid: "Ungültiges Datum",
          future: "Sie müssen ein zukünftiges Datum auswählen"
        }
      }
    },
    roles: {
      title: "Rollen",
      list: "Liste",
      new: "Neu",
      edit: "Bearbeiten",
      fields: {
        id: "ID",
        name: "Name",
        description: "Beschreibung",
        process: "Prozess"
      },
      delete: {
        title: "Rolle löschen",
        message: "Möchten Sie die Rolle löschen?"
      },
      validation: {
        name: {
          required: "Rollenname ist erforderlich",
          maxLength: "Rollenname muss weniger als {max} Zeichen lang sein"
        },
        description: {
          maxLength: "Beschreibung muss weniger als {max} Zeichen lang sein"
        }
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
    save: "Speichern",
    reset: "Zurücksetzen",
    update: "Aktualisieren",
    edit: "Bearbeiten Informationen",
    close: "Schließen",
    cancel: "Stornieren",
    verify: "Überprüfen",
    deleted: "Erfolgreich gelöscht",
    basicInformation: "Grundlegende Informationen",
    additionalParameters: "Zusätzliche Parameter"
  },
  $vuetify: {
    ...de
  }
};
