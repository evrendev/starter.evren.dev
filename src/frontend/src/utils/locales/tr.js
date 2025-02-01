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
    TwoFactorAuthentication: {
      subtitle: "Authenticator uygulamasından kodu girin",
      label: "Kod",
      placeholder: "Kodunuzu girin",
      required: "Kodunuzu girmeniz gerekmektedir",
      submit: "Doğrula"
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
        settings: "Ayarlar",
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
    starter: {
      title: "Başlangıç",
      subtitle: "Yönetim Paneline Hoşgeldiniz"
    },
    audits: {
      title: "İşlem Geçmişi",
      list: "Liste",
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
    close: "Kapat",
    deleted: "Başarıyla silindi",
    basicInformation: "Temel Bilgiler",
    additionalParameters: "Ek Parametreler"
  },
  $vuetify: {
    ...tr
  }
};
