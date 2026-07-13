# Frontend Paket Haritası

| Paket                                                             | Kullanım Amacı                                                                          |
| ----------------------------------------------------------------- | --------------------------------------------------------------------------------------- |
| Vue 3 + TypeScript                                                | Ana framework                                                                           |
| Vuetify 3 (+ vite-plugin-vuetify)                                 | UI component kütüphanesi — özel CSS'ten önce Vuetify ile çözülebiliyor mu kontrol et    |
| Pinia (+ @pinia/testing)                                          | State management                                                                        |
| Axios                                                             | HTTP client                                                                             |
| vue-router (+ vite-plugin-vue-layouts)                            | Routing, layout bazlı sayfa yapısı                                                      |
| vee-validate + yup (+ @vee-validate/yup)                          | Form validation                                                                         |
| vue-i18n                                                          | Çoklu dil desteği                                                                       |
| **reveal.js**                                                     | **LessonPage (slide) oynatıcı — KARAR VERİLDİ, kullanılacak**                           |
| **quill / vue-quilly** (+ quill-resize-image, quill-table-better) | **LessonPage.Content için WYSIWYG editör (admin tarafı)**                               |
| @vueuse/core, @vueuse/math                                        | Composable utility'ler                                                                  |
| @floating-ui/dom, @popperjs/core                                  | Tooltip/popover pozisyonlama                                                            |
| date-fns (+ @date-io/date-fns)                                    | Tarih işlemleri                                                                         |
| vue-flatpickr-component                                           | Date picker                                                                             |
| vue3-apexcharts                                                   | Grafik (progress dashboard, course analytics için kullanılabilir)                       |
| vue3-perfect-scrollbar                                            | Özel scrollbar (GitLab University'deki sol Lessons panelindeki scroll gibi)             |
| vue-json-pretty                                                   | JSON görüntüleme (debug/admin panel)                                                    |
| prismjs, vue-prism-component                                      | Kod syntax highlighting (LessonPage içinde kod bloğu gösterimi için)                    |
| @fancyapps/ui                                                     | Lightbox/galeri (LessonPage içi görsel büyütme)                                         |
| qrcode                                                            | QR kod üretimi (2FA setup ekranı, backend'deki Otp.Net ile eşleşiyor)                   |
| webfontloader, roboto-fontface                                    | Font yükleme                                                                            |
| bootstrap                                                         | Muhtemelen legacy/yardımcı, Vuetify ana sistem — yeni component'lerde Vuetify tercih et |
| Iconify (@iconify/vue + icon setleri: bx, bxl, bxs, fa, mdi)      | İkon sistemi                                                                            |

## Test & Kalite

| Paket                                                                           | Amaç                |
| ------------------------------------------------------------------------------- | ------------------- |
| @playwright/test, eslint-plugin-playwright                                      | E2E test            |
| @vue/test-utils, @testing-library/vue                                           | Component/unit test |
| ESLint (+ @nuxt/eslint-config, typescript-eslint, unicorn, sonarjs, regexp vb.) | Lint                |
| Stylelint (+ idiomatic-order, standard-scss)                                    | CSS/SCSS lint       |
| Prettier                                                                        | Format              |

## Doğrulanmış Konvansiyonlar (frontend keşfi — kesin)

**Routing**: `src/plugins/router/index.ts` + `routes.ts` (domain bazlı böl: `routes.auth.ts`,
`routes.admin.ts`, `routes.public.ts`). `beforeEach` guard: `requiresAuth` meta + `personalStore.hasPermission()`.

**Auth/JWT**: Token `localStorage`'da (`accessToken`, `refreshToken`, `refreshTokenExpiryTime`).
Merkezi axios instance: `src/utils/http.ts` — request'e `Authorization: Bearer` + `TenantId` +
`Accept-Language` header'ları otomatik ekleniyor. 401'de `authStore.refresh()` ile otomatik retry
(max 3 deneme).

**Pinia Store Şablonu (Options API, kritik — Composition API DEĞİL)**:

```typescript
defineStore("name", {
  state: () => ({ loading, error, items, filters, pagination }),
  actions: {
    async getAllItems() { return await handleRequest<T>(http.get(...)) },
    async getPaginatedItems() { ... },
    setFilters() { ... }
  }
})
```

**Ayrı bir service/API katmanı YOK — store'lar doğrudan API katmanı.** `handleRequest<T>()` wrapper
promise'i `Result<T>` (succeeded/data/errors) tipine çeviriyor. Mevcut store örnekleri: course,
lesson, chapter, category, user, role, personal, tenant, notification, app.

**Vuetify**: Custom tema `src/assets/styles/admin/variables/_vuetify.scss`, `vite-plugin-vuetify`
ile bağlı. Ayrıca `date.ts`, `locale.ts`, `icons.ts` config dosyaları.

**Auto-import**: Component'ler `src/@core/components` ve `src/components`'ten otomatik register
ediliyor — yeni component için manuel import gerekmiyor.

**Mevcut Course/Lesson kodu (LessonPage için ŞABLON)**:

- `src/models/course.ts`, `src/models/lesson.ts`
- `src/types/requests/{course,lesson}.ts`, `src/types/responses/...`
- `src/stores/course.ts`, `src/stores/lesson.ts` (tam implement)
- `src/pages/admin/chapters/form.vue` (nested form örneği)

**Eksik olan (bizim inşa edeceğimiz)**: LessonPage player UI, LessonPage CRUD (list/form),
LessonPageProgress UI, Mark Complete akışı.

## LessonPage Player Tasarım Kararı — NETLEŞTİ: reveal.js

Karar verildi: **reveal.js** ile gerçek slide-deck deneyimi kurulacak (native geçiş animasyonu,
klavye navigasyonu). Dikkat edilecekler:

- reveal.js kendi CSS temasını getiriyor — Vuetify'ın custom teması (`_vuetify.scss`) ile çakışma
  riski var, slide container'ı Vuetify layout'undan izole (örn. tam ekran/`v-dialog` container
  içine) yerleştirmek gerekebilir.
- Slide içeriği backend'den async geldiği için `Reveal.initialize()` DOM'a slide'lar
  basıldıktan SONRA (`nextTick`) çağrılmalı.
- reveal.js 5.x'te tam `destroy()` desteği sınırlı — component unmount'ta memory leak riski,
  Task F5'te bu netleştirilecek/test edilecek.
- Slide değişimi (`slidechanged` event) → `markPageCompleted` store action'ını tetikleyecek.
