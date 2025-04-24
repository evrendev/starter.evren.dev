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
      donations: {
        title: "Bağışlar",
        fountains: {
          title: "Kuyu Bağışları",
          all: "Tüm Bağışlar",
          bks: "BKS",
          bgs: "BGS",
          aki: "AKI",
          agi: "AGI"
        },
        new: "Yeni Bağış"
      },
      todos: {
        header: "Görevler",
        title: "Görev Listesi"
      },
      admin: {
        title: "Yönetici İşlemleri"
      },
      users: {
        title: "Kullanıcılar",
        list: "Kullanıcı Listesi",
        new: "Kullanıcı Ekle"
      },
      roles: {
        title: "Roller",
        list: "Rol Listesi",
        new: "Rol Ekle"
      },
      tenants: {
        title: "Kurumlar",
        list: "Kurum Listesi",
        new: "Kurum Ekle"
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
    donations: {
      title: "Bağışlar",
      list: "Bağış Listesi",
      new: "Yeni Bağış",
      edit: "Bağış Düzenle",
      info: "Bilgi",
      delete: {
        title: "Bağışı Sil",
        message: "Bağışı silmek istediğinizden emin misiniz?"
      },
      fountains: {
        title: "Kuyu Bağışları",
        addEmptyDonation: "Boş Bağış Ekle",
        fields: {
          id: "ID",
          contact: "Bağışçı",
          phone: "Telefon",
          creationDate: "Tarih",
          banner: "Afiş",
          projectCode: "Proje Kodu",
          info: "Bilgi",
          weeks: "Hafta",
          status: "Durum",
          team: "Takım",
          media: "Medya",
          detail: "İşlem"
        },
        team: {
          none: "Yok",
          morteza: "Morteza Takımı",
          idris: "Idris Takımı"
        },
        status: {
          none: "Yok",
          initialWeek: "Başlangıç Haftası",
          ongoingEarlyWeeks: "Devam Eden İlk Haftalar",
          week5Media: "5. Hafta Medya",
          week6Warning: "6. Hafta Uyarı",
          week8Critical: "8. Hafta Kritik",
          published: "Yayınlandı"
        },
        projectCodes: {
          bks: "BKS",
          bgs: "BGS",
          aki: "AKI",
          agi: "AGI"
        },
        details: {
          creationDate: "Tarih",
          weeks: "Hafta",
          team: "Takım",
          contact: "Bağışçı",
          phone: "Telefon",
          project: "Proje",
          projectCode: "Proje Kodu",
          projectNumber: "Proje Numarası",
          banner: "Afiş",
          mediaStatus: "Medya",
          mediaInformation: "Medya Bilgisi",
          transactionId: "İşlem ID",
          source: "Kaynak",
          link: "Bağış Linki"
        },
        validation: {
          contact: {
            required: "Bağışçı bilgisini girmeniz gerekmektedir.",
            maxLength: "Bağışçı bilgisi en fazla {max} karakter olmalıdır."
          },
          phone: {
            required: "İletişim bilgisini girmeniz gerekmektedir.",
            maxLength: "İletişim bilgisi en fazla {max} karakter olmalıdır."
          },
          banner: {
            required: "Afiş bilgisini girmeniz gerekmektedir.",
            maxLength: "Afiş bilgisi en fazla {max} karakter olmalıdır."
          },
          creationDate: {
            required: "Bağış tarihini girmeniz gerekmektedir.",
            invalid: "Bağış tarihi geçerli bir formatta değil."
          },
          projectCode: {
            required: "Proje kodunu girmeniz gerekmektedir.",
            invalid: "Proje kodu geçerli bir proje kodu değil."
          }
        }
      },
      media: {
        title: "Medya",
        change: "Medya Durumu Değiştir",
        status: {
          none: "Hiçbiri",
          missing: "Eksik",
          arrived: "Geldi",
          edited: "Düzenlendi",
          online: "Yayınlandı",
          transferred: "Gönderildi",
          reviewed: "Gözden Geçiriliyor"
        }
      }
    },
    tenants: {
      title: "Kurumlar",
      list: "Kurum Listesi",
      new: "Yeni Kurum",
      edit: "Kurum Düzenle",
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
      delete: {
        title: "Kurum Sil",
        message: "Kurumu silmek istediğinizden emin misiniz?"
      },
      restore: {
        title: "Kurumu Geri Yükle",
        message: "Kurumu geri yüklemek istediğinizden emin misiniz?"
      },
      activate: {
        title: "Kurumu Etkinleştir",
        message: "Kurumu etkinleştirmek istediğinizden emin misiniz?"
      },
      deactivate: {
        title: "Kurumu Devre Dışı Bırak",
        message: "Kurumu devre dışı bırakmak istediğinizden emin misiniz?"
      },
      validation: {
        name: {
          required: "Kurum adı girmeniz gerekmektedir",
          maxLength: "Kurum adı en fazla {max} karakter olabilir"
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
      list: "Rol Listesi",
      new: "Yeni Rol",
      edit: "Rol Düzenle",
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
    },
    users: {
      title: "Kullanıcılar",
      list: "Kullanıcı Listesi",
      new: "Yeni Kullanıcı",
      edit: "Kullanıcı Düzenle",
      delete: {
        title: "Kullanıcıyı Sil",
        message: "Kullanıcıyı silmek istediğinizden emin misiniz?",
        options: {
          true: "Evet",
          false: "Hayır"
        }
      },
      restore: {
        title: "Kullanıcıyı Geri Yükle",
        message: "Kullanıcıyı geri yüklemek istediğinizden emin misiniz?"
      },
      fields: {
        initial: "#",
        twoFactorEnabled: "2FA",
        gender: "Cinsiyet",
        firstName: "Ad",
        lastName: "Soyad",
        email: "E-posta",
        jobTitle: "Ünvan",
        process: "İşlem",
        language: "Dil",
        password: "Şifre",
        confirmPassword: "Şifreyi Onayla"
      },
      validation: {
        gender: {
          required: "Cinsiyet seçmeniz gerekmektedir"
        },
        email: {
          required: "E-posta adresi girmeniz gerekmektedir",
          invalid: "Geçersiz e-posta adresi"
        },
        firstName: {
          required: "Ad girmeniz gerekmektedir",
          maxLength: "Ad en fazla {max} karakter olabilir"
        },
        lastName: {
          required: "Soyad girmeniz gerekmektedir",
          maxLength: "Soyad en fazla {max} karakter olabilir"
        },
        password: {
          required: "Şifre girmeniz gerekmektedir",
          minLength: "Şifre en az {min} karakter olmalıdır",
          uppercase: "Şifre en az bir büyük harf içermelidir",
          lowercase: "Şifre en az bir küçük harf içermelidir",
          number: "Şifre en az bir rakam içermelidir",
          special: "Şifre en az bir özel karakter içermelidir"
        },
        confirmPassword: {
          required: "Şifreyi onaylamanız gerekmektedir",
          match: "Şifreler eşleşmiyor"
        },
        jobTitle: {
          maxLength: "Ünvan en fazla {max} karakter olabilir"
        },
        permissions: {
          required: "En az bir izin seçmelisiniz"
        }
      },
      helpers: {
        information: "Kullanıcı Bilgileri",
        permissions: "İzinler"
      }
    }
  },
  error: {
    title: "Bir şeyler ters gitti!",
    description: "Aradığınız sayfa taşınmış, kaldırılmış, yeniden adlandırılmış veya hiç var olmamış olabilir!",
    home: "Anasayfa"
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
    delete: "Sil",
    deleted: "Başarıyla silindi",
    selectAll: "Hepsini Seç",
    error: "Hata",
    navigation: "Navigasyon",
    goToTop: "Başa Dön",
    showDeletedItems: "Silinmiş Öğeleri Göster",
    showActiveItems: "Aktif Öğeleri Göster",
    copy: "Kopyala",
    copied: "Kopyalandı",
    showDetails: "Detayları Göster",
    openWhatsapp: "WhatsApp Aç",
    changeTeam: "Takım Değiştir",
    openWhatsapp: "WhatsApp Aç",
    select: "Bir Seçenek Seçin",
    changeMediaStatus: "Medya Durumunu Değiştir",
    modules: {
      donations: "Bağışlar",
      todos: "Görevler",
      tenants: "Kurumlar",
      roles: "Roller",
      images: "Görseller",
      users: "Kullanıcılar",
      audits: "İşlem Geçmişi",
      files: "Dosyalar"
    },
    permissions: {
      read: "Oku",
      create: "Oluştur",
      delete: "Sil",
      restore: "Geri Yükle",
      edit: "Düzenle"
    },
    active: "Aktif",
    passive: "Pasif",
    true: "Evet",
    false: "Hayır"
  },
  $vuetify: {
    ...tr
  }
};
