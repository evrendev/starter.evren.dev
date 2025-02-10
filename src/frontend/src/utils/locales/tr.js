import { tr } from "vuetify/locale";

export default {
  auth: {
    login: {
      title: "Giriş Yap",
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
      recaptcha: {
        label: "Ben bir robot değilim",
        required: "Recaptcha doğrulaması gerekmektedir",
        invalid: "Recaptcha doğrulaması başarısız oldu"
      },
      rememberMe: "Beni Hatırla",
      forgotPassword: "Şifrenizi mi unuttunuz?",
      submit: "Giriş Yap",
      resetForm: "Sıfırla"
    },
    twoFactorAuth: {
      subtitle: "Authenticator uygulamasından kodu girin",
      label: "Kod",
      placeholder: "Kodunuzu girin",
      required: "Kodunuzu girmeniz gerekmektedir",
      submit: "Doğrula",
      code: {
        required: "Kod girmeniz gerekmektedir",
        invalid: "Geçersiz kod"
      }
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
      },
      profile: {
        title: "Kullanıcı Bilgilerim",
        logout: "Çıkış Yap",
        goodMorning: "Günaydın",
        goodAfternoon: "İyi Günler",
        goodEvening: "İyi Akşamlar"
      }
    },
    footerPanel: {
      home: "Anasayfa",
      documentation: "Dökümantasyon",
      support: "Destek"
    },
    sidebar: {
      dashboard: {
        header: "Yönetim Paneli",
        title: "Anasayfa"
      },
      todos: {
        header: "Görevler",
        title: "Liste"
      },
      admin: {
        title: "Yönetici"
      },
      users: {
        title: "Kullanıcılar",
        list: "Liste",
        new: "Yeni"
      },
      roles: {
        title: "Roller",
        list: "Liste",
        new: "Yeni"
      },
      tenants: {
        title: "Kiracılar",
        list: "Liste",
        new: "Yeni"
      },
      audits: {
        title: "İşlem Geçmişi"
      }
    }
  },
  messages: {
    success: "İşlem başarılı",
    error: "İstek başarısız"
  },
  admin: {
    dashboard: {
      title: "Yönetim Paneli",
      subtitle: "Yönetim Paneline Hoşgeldiniz"
    },
    profile: {
      title: "Profil",
      email: "E-posta",
      firstName: "Ad",
      lastName: "Soyad",
      gender: "Cinsiyet",
      jobTitle: "Ünvan",
      language: "Dil",
      validation: {
        firstName: {
          required: "Ad girmeniz gerekmektedir",
          maxLength: "Ad en fazla {max} karakter olabilir"
        },
        lastName: {
          required: "Soyad girmeniz gerekmektedir",
          maxLength: "Soyad en fazla {max} karakter olabilir"
        },
        jobTitle: {
          maxLength: "Ünvan en fazla {max} karakter olabilir"
        },
        email: {
          required: "E-posta adresi girmeniz gerekmektedir",
          invalid: "Geçersiz e-posta adresi"
        },
        language: {
          required: "Dil seçmeniz gerekmektedir"
        },
        gender: {
          required: "Cinsiyet seçmeniz gerekmektedir"
        },
        twoFactorAuth: {
          code: {
            required: "Doğrulama kodu girmeniz gerekmektedir",
            invalid: "Geçersiz doğrulama kodu"
          }
        }
      },
      twoFactorAuth: {
        title: "İki Faktörlü Kimlik Doğrulama",
        description:
          "İki faktörlü kimlik doğrulama, hesabınızı daha güvenli hale getirir. Hesabınıza giriş yaparken, doğrulama kodunu girerek kimliğinizi doğrulamanız gerekir.",
        enable: "İki Faktörlü Kimlik Doğrulamayı Etkinleştir",
        disable: "İki Faktörlü Kimlik Doğrulamayı Devre Dışı Bırak",
        confirmDisable: {
          title: "İki Faktörlü Kimlik Doğrulamayı Devre Dışı Bırak",
          message:
            "İki faktörlü kimlik doğrulamayı devre dışı bırakmak istediğinizden emin misiniz? Bu, hesabınızı daha az güvenli hale getirecektir."
        },
        setup: {
          title: "İki Faktörlü Kimlik Doğrulamayı Ayarla",
          intro: "İki faktörlü kimlik doğrulamayı etkinleştirmek için, aşağıdaki adımları izleyin:",
          start: "Başla",
          manual: "Veya bu kodu manuel olarak kimlik doğrulama uygulamanıza girin:",
          code: "Doğrulama Kodu"
        }
      }
    },
    audits: {
      title: "İşlem Geçmişi",
      list: "Liste",
      edit: "Düzenle",
      fields: {
        id: "ID",
        user: "Kullanıcı",
        dateTime: "Tarih/Zaman",
        action: "İşlem",
        entityType: "Varlık Türü",
        detail: "Detay"
      },
      actions: {
        insert: "Ekle",
        update: "Güncelle",
        delete: "Sil",
        recovered: "Kurtar"
      },
      details: {
        title: "İşlem Detayı"
      }
    },
    tenants: {
      title: "Kiracılar",
      list: "Liste",
      new: "Yeni",
      fields: {
        id: "ID",
        name: "Ad",
        adminEmail: "E-posta",
        validUntil: "Bitiş Tarihi",
        isActive: "Aktif Mi",
        process: "İşlem",
        description: "Açıklama",
        connectionString: "Bağlantı Dizesi",
        host: "Sunucu"
      },
      activeOptions: {
        true: "Aktif",
        false: "Pasif"
      },
      delete: {
        title: "Kiracıyı Sil",
        message: "Kiracıyı silmek istediğinizden emin misiniz?"
      },
      activate: {
        title: "Kiracıyı Etkinleştir",
        message: "Kiracıyı etkinleştirmek istediğinizden emin misiniz?"
      },
      deactivate: {
        title: "Kiracıyı Devre Dışı Bırak",
        message: "Kiracıyı devre dışı bırakmak istediğinizden emin misiniz?"
      },
      validation: {
        name: {
          required: "Kiracı adı girmeniz gerekmektedir",
          maxLength: "Kiracı adı en fazla {max} karakter olabilir"
        },
        description: {
          maxLength: "Açıklama en fazla {max} karakter olabilir"
        },
        adminEmail: {
          required: "Eposta adresi girmeniz gerekmektedir",
          invalid: "Geçersiz e-posta adresi"
        },
        validUntil: {
          required: "Bitiş tarihi girmeniz gerekmektedir",
          invalid: "Geçersiz tarih",
          future: "Gelecek bir tarih seçmelisiniz"
        }
      }
    },
    roles: {
      title: "Roller",
      list: "Liste",
      new: "Yeni",
      fields: {
        id: "ID",
        name: "Ad",
        description: "Açıklama",
        process: "İşlem"
      },
      delete: {
        title: "Rolü Sil",
        message: "Rolü silmek istediğinizden emin misiniz?"
      },
      validation: {
        name: {
          required: "Rol adı girmeniz gerekmektedir",
          maxLength: "Rol adı en fazla {max} karakter olabilir"
        },
        description: {
          maxLength: "Açıklama en fazla {max} karakter olabilir"
        }
      }
    }
  },
  common: {
    search: "Ara",
    filters: "Filtreler",
    action: "İşlem",
    startDate: "Başlangıç Tarihi",
    endDate: "Bitiş Tarihi",
    selectDate: "Tarih Seç",
    all: "Hepsi",
    submit: "Gönder",
    save: "Kaydet",
    reset: "Sıfırla",
    edit: "Bilgileri Düzenle",
    update: "Güncelle",
    close: "Kapat",
    cancel: "Vazgeç",
    verify: "Doğrula",
    confirm: "Onayla",
    deleted: "Başarıyla silindi",
    basicInformation: "Temel Bilgiler",
    additionalParameters: "Ek Parametreler"
  },
  $vuetify: {
    ...tr
  }
};
