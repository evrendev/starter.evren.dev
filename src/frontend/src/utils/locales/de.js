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
      donations: {
        title: "Spenden",
        fountains: {
          title: "Brunnen Spenden",
          all: "Alle Spenden",
          bks: "BKS",
          bgs: "BGS",
          aki: "AKI",
          agi: "AGI"
        },
        new: "Neu Spende"
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
        confirmDisable: {
          title: "Zwei-Faktor-Authentifizierung deaktivieren",
          message: "Möchten Sie die Zwei-Faktor-Authentifizierung deaktivieren? Dies macht Ihr Konto weniger sicher."
        },
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
    donations: {
      title: "Spenden",
      list: "Liste",
      new: "Neue Spende",
      edit: "Bearbeiten",
      info: "Info",
      delete: {
        title: "Spende löschen",
        message: "Möchten Sie die Spende löschen?"
      },
      fountains: {
        title: "Brunnen Spenden",
        fields: {
          id: "ID",
          contact: "Kontakt",
          phone: "Telefon",
          banner: "Banner",
          creationDate: "Datum",
          info: "Info",
          projectCode: "Projektcode",
          weeks: "Wochen",
          status: "Status",
          team: "Team",
          media: "Medien",
          detail: "Aktion"
        },
        projectCodes: {
          bks: "BKS",
          bgs: "BGS",
          aki: "AKI",
          agi: "AGI"
        },
        status: {
          none: "Keine",
          initialWeek: "Initiale Woche",
          ongoingEarlyWeeks: "Laufende frühe Wochen",
          week5Media: "Woche 5 Medien",
          week6Warning: "Woche 6 Warnung",
          week8Critical: "Woche 8 kritisch",
          published: "Veröffentlicht"
        },
        details: {
          creationDate: "Datum",
          weeks: "Wochen",
          team: "Team",
          contact: "Kontakt",
          phone: "Telefon",
          project: "Projekt",
          projectCode: "Projektcode",
          projectNumber: "Projektnummer",
          banner: "Banner",
          mediaStatus: "Medien",
          transactionId: "Transaktions-ID",
          source: "Quelle",
          link: "Link"
        },
        validation: {
          contact: {
            required: "Kontakt ist erforderlich",
            maxLength: "Kontakt muss weniger als {max} Zeichen lang sein"
          },
          phone: {
            required: "Telefon ist erforderlich.",
            maxLength: "Telefon muss weniger als {max} Zeichen lang sein"
          },
          banner: {
            required: "Banner ist erforderlich.",
            maxLength: "Banner muss weniger als {max} Zeichen lang sein"
          },
          creationDate: {
            required: "Datum ist erforderlich.",
            invalid: ""
          },
          projectCode: {
            required: "Projektcode ist erforderlich.",
            invalid: "Proje kodu geçerli bir proje kodu değil."
          }
        }
      },
      media: {
        title: "Medien",
        status: {
          none: "Keine",
          missing: "Fehlend",
          arrived: "Ist gekommen",
          edited: "Geschnitten",
          online: "Online",
          transferred: "Transferiert",
          reviewed: "Kontrolliert"
        }
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
      delete: {
        title: "Mandant löschen",
        message: "Möchten Sie den Mandanten löschen?"
      },
      restore: {
        title: "Mandant wiederherstellen",
        message: "Möchten Sie den Mandanten wiederherstellen?"
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
    },
    users: {
      title: "Benutzer",
      list: "Liste",
      new: "Neu",
      edit: "Bearbeiten",
      delete: {
        title: "Benutzer löschen",
        message: "Möchten Sie den Benutzer löschen?",
        options: {
          true: "Ja",
          false: "Nein"
        }
      },
      restore: {
        title: "Benutzer wiederherstellen",
        message: "Möchten Sie den Benutzer wiederherstellen?"
      },
      fields: {
        initial: "#",
        twoFactorEnabled: "2FA",
        gender: "Geschlecht",
        firstName: "Vorname",
        lastName: "Nachname",
        email: "E-Mail",
        process: "Prozess",
        language: "Sprache",
        password: "Passwort",
        confirmPassword: "Passwort bestätigen"
      },
      validation: {
        gender: {
          required: "Geschlecht ist erforderlich"
        },
        email: {
          required: "E-Mail ist erforderlich",
          invalid: "Ungültige E-Mail-Adresse"
        },
        firstName: {
          required: "Vorname ist erforderlich",
          maxLength: "Vorname muss weniger als {max} Zeichen lang sein"
        },
        lastName: {
          required: "Nachname ist erforderlich",
          maxLength: "Nachname muss weniger als {max} Zeichen lang sein"
        },
        password: {
          required: "Passwort ist erforderlich",
          minLength: "Passwort muss mindestens {min} Zeichen lang sein",
          uppercase: "Passwort muss mindestens einen Großbuchstaben enthalten",
          lowercase: "Passwort muss mindestens einen Kleinbuchstaben enthalten",
          number: "Passwort muss mindestens eine Zahl enthalten",
          special: "Passwort muss mindestens ein Sonderzeichen enthalten"
        },
        confirmPassword: {
          required: "Passwort bestätigen ist erforderlich",
          match: "Passwörter stimmen nicht überein"
        },
        jobTitle: {
          maxLength: "Berufsbezeichnung muss weniger als {max} Zeichen lang sein"
        },
        permissions: {
          required: "Berechtigungen sind erforderlich"
        }
      },
      helpers: {
        information: "Benutzerinformationen",
        permissions: "Benutzerberechtigungen"
      }
    }
  },
  error: {
    title: "Etwas ist schief gelaufen!",
    description: "Die Seite, die Sie suchen, existiert nicht oder ist verschoben worden.",
    home: "Startseite"
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
    confirm: "Bestätigen",
    deleted: "Erfolgreich gelöscht",
    basicInformation: "Grundlegende Informationen",
    additionalParameters: "Zusätzliche Parameter",
    selectAll: "Wählen Sie alle",
    error: "Fehler",
    navigation: "Navigation",
    goToTop: "Gehe nach oben",
    showDeletedItems: "Gelöschte Elemente anzeigen",
    showActiveItems: "Aktive Elemente anzeigen",
    copy: "Kopieren",
    copied: "Kopiert",
    showDetails: "Details anzeigen",
    modules: {
      donations: "Spenden",
      todos: "Todos Listen",
      tenants: "Mandanten",
      roles: "Rollen",
      images: "Bilder",
      users: "Benutzer",
      audits: "Audits",
      files: "Dateien"
    },
    permissions: {
      read: "Lesen",
      create: "Erstellen",
      delete: "Löschen",
      restore: "Wiederherstellen",
      edit: "Bearbeiten"
    },
    active: "Aktiv",
    passive: "Passiv",
    true: "Ja",
    false: "Nein"
  },
  $vuetify: {
    ...de
  }
};
