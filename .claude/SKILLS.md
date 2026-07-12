# SKILLS — Senaryo Bazlı Checklist

Bu dosya bir "ne zaman ne yapılır" tablosudur. İlgili senaryoyu bul, sırasıyla uygula.

## 0) Yeni Application Feature'a Başlamadan Önce — Konvansiyon (DOĞRULANMIŞTUR)

**Kanonik Pattern = Lessons feature**. Tüm features (Products, Chapters, vb.) aynı deseni takip ediyor:

**Kesin Kurallar:**
1. **Klasör Yapısı**: `Lessons/Queries/{Create,Update,Delete,Get,Paginate,Export,Search}/` + `Entities/` + `EventHandlers/` + `Specifications/`
2. **Request Naming**: `CreateLessonRequest`, `UpdateLessonRequest`, vs. (NOT Command, NOT Query)
   - Interface: `IRequest<T>` (MediatR)
   - Handler: `CreateLessonRequestHandler` (aynı dosyada Request + Validator ile)
3. **Repository Injection**:
   - Write ops: `IRepository<T>`
   - Read ops: `IReadRepository<T>` (ya da `IRepository<T>`)
   - **NOT** `ApplicationDbContext` direkt
4. **Domain Events**: Handler'da manuel event ekleme YAZMA — `EventAddingRepositoryDecorator<T>`
   otomatik ekliyor. Decorator her Add/Update/Delete'de event inject eder, sonra SaveChangesAsync'de
   BaseDbContext tarafından publish edilir.
5. **Specification**: `Ardalis.Specification<TEntity, TProjection>` — TProjection (DTO) otomatik
   map edilir.
6. **Pagination**: `PaginationResponse<T>` class'ını kendi tipini icat etme.

**Şablon olarak kullan**: `src/backend/Core/Application/Catalog/Lessons/` (tüm adımlar için)

## 1) Yeni Domain Entity Ekleme

1. Benzer bir mevcut entity'yi şablon al (örn. `Course.cs`) — `AuditableEntity, IAggregateRoot`,
   private setter, ctor, `Update()` metodu.
2. `IEntityTypeConfiguration<T>` sınıfı yaz (`Infrastructure/Persistence/Configuration/`).
   Course zincirindeyse `builder.IsMultiTenant()` ekle, ilişki tablosuysa ekleme.
3. `ApplicationDbContext`'e `DbSet<T>` ekle.
4. Migration oluştur (`dotnet ef migrations add ...`) — **çalıştırma, onay bekle**.

## 2) Yeni CQRS Feature Ekleme (Application katmanı)

Klasör deseni — `Application/Catalog/{Feature}/`:

```
Entities/          → {Name}Dto.cs, {Name}DetailsDto.cs, {Name}ExportDto.cs
EventHandlers/      → {Name}CreatedEventHandler.cs, ...UpdatedEventHandler.cs, ...DeletedEventHandler.cs
Queries/
  ├── Create/       → Create{Name}Request + Handler + Validator (single file)
  ├── Update/       → Update{Name}Request + Handler + Validator (single file)
  ├── Delete/       → Delete{Name}Request + Handler (single file)
  ├── Get/          → Get{Name}Request + Handler (+ GetAll{Name}Request + Handler)
  ├── Paginate/     → Paginate{Name}Filter + Handler
  ├── Export/       → Export{Name}Request + Handler (ClosedXML ile Excel)
  └── Search/       → Search{Name}Request + Handler (opsiyonel, pagination variant)
Specifications/     → Ardalis.Specification<TEntity, TProjection> tabanlı spec sınıfları
```

**Adımlar:**

1. Şablon: `src/backend/Core/Application/Catalog/Lessons/` — dosya dosya kopyala.
2. **Repository**: Write ops → `IRepository<T>`, Read ops → `IReadRepository<T>` inject.
3. **Validator**: FluentValidation, `CustomValidator<TRequest>` base class, aynı dosyada handler ile.
4. **Mapping**: `.Adapt<T>()` otomatik (Specification'da TProjection prop define eder).
5. **Specification**: `Specification<TEntity, TProjection>` — filter/include mevcut spec'lerden örnek al.
6. **Pagination**: `PaginationResponse<T>` class'ı kullan, kendi response tipi yazma.
7. **Domain Events**:
   - **Handler içinde event ekleme YAPMA** — `EventAddingRepositoryDecorator<T>` otomatik ekliyor.
   - Event handler yazılacaksa: `EventNotificationHandler<EntityCreatedEvent<T>>` implement et.
   - BaseDbContext.SendDomainEventsAsync event'leri otomatik publish eder.

## 3) Progress / Rollup Mantığı Eklerken

Asla command handler içine rollup hesaplama yazma. Zincir şu şekilde olmalı:

```
MarkXCompletedCommand → ilgili Progress kaydını günceller → XCompletedEvent fırlatır
   → RecalculateParentProgressHandler (ayrı dosya, notification handler) üst seviyeyi günceller
```

Detay: `docs/lms-domain.md`.

## 4) Yeni API Endpoint

Mevcut route grubu/controller'ı (Lesson için ne kullanılıyorsa) şablon al. Versioning
(`Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer`) ve NSwag doc attribute'larını koru.

## 5) Yeni Vue Sayfası/Component

1. Vuetify component'leri kullan, custom CSS'ten önce Vuetify tema/prop ile çözülebiliyor mu bak.
2. Form varsa: `vee-validate` + `yup` şeması, mevcut bir form component'i şablon al.
3. API çağrısı: Axios instance (mevcut `services/` veya `api/` katmanı neyse onu kullan), doğrudan
   component içinde `axios.get(...)` yazma.
4. State: Pinia store, domain bazlı (`useCourseStore`, `useLessonPlayerStore` gibi).
5. Çeviri: `vue-i18n` kullanılıyorsa hardcoded string bırakma, mevcut locale dosyasına ekle.
6. Route: `vue-router`, otomatik route generation varsa (`vite-plugin-vue-layouts`) klasör
   konvansiyonuna uy.

## 6) LessonPage Player (Slide Oynatıcı) — Özel Not

`reveal.js` frontend'de dependency olarak zaten mevcut. Player UI'ı için iki seçenek var:

- **A**: reveal.js ile gerçek "slide deck" deneyimi (geçiş animasyonları, klavye navigasyonu native gelir)
- **B**: Vuetify tabanlı custom stepper/carousel (GitLab University'nin sade "Next Page" tarzına daha yakın)

Karar netleşene kadar varsayım yapma, kullanıcıya sor. Detay: `docs/frontend-stack.md`.

## 7) İçerik Editörü (Admin — LessonPage.Content yazarken)

`quill` + `vue-quilly` (+ `quill-resize-image`, `quill-table-better`) zaten dependency. Yeni bir
WYSIWYG editör kütüphanesi ekleme, bunları kullan.
