# SKILLS — Senaryo Bazlı Checklist

Bu dosya bir "ne zaman ne yapılır" tablosudur. İlgili senaryoyu bul, sırasıyla uygula.

## 0) Yeni Application Feature'a Başlamadan Önce — Konvansiyon Doğrulama (ZORUNLU)

`Products` ve `Lessons` feature'larında farklı pattern izleri var (`Request/RequestHandler` +
`Queries/` alt klasörü vs. `Create/Update/Delete/Get/Paginate/Export` düz klasörleri). Yeni kod
yazmadan önce:

1. `Lessons/` feature'ının TÜM dosyalarını oku (bu bizim ana referansımız, çünkü ekran görüntüsünde
   görülen yapı bu).
2. Handler constructor'larına bak: `IRepository<T>` mi, `IReadRepositoryBase<T>` mi, yoksa doğrudan
   `ApplicationDbContext` mi inject ediliyor?
3. Command/Query mi yoksa Request mi deniyor — sınıf isimlerine ve `IRequest<T>`/`IQuery<T>` gibi
   base interface'lere bak.
4. Specification sınıfları `Ardalis.Specification.Specification<T>`'ten mi türüyor, nasıl kullanılıyor?
5. Bulguları özetle, SONRA kod yazmaya başla. Varsayımda bulunma.

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
Create/             → CreateXCommand + Handler + Validator
Update/             → UpdateXCommand + Handler + Validator
Delete/              → DeleteXCommand + Handler
Get/                → GetXQuery + Handler
Paginate/            → PaginateXQuery + Handler
Export/               → ExportXQuery + Handler (ClosedXML ile Excel)
Specifications/        → Ardalis.Specification tabanlı spec sınıfları
```

Adımlar:

1. En yakın mevcut feature'ı (örn. `Lessons/`) referans al, dosya dosya birebir örnek çıkar.
2. Handler'da veri erişimi: §0'da doğrulanan pattern neyse onu kullan (`IRepository<T>` muhtemel,
   doğrudan `DbContext` DEĞİL — ama önce doğrula).
3. Validator: FluentValidation, mevcut bir `Validator.cs` dosyasını şablon al.
4. Mapping: Mapster kullan (`.Adapt<T>()`), mevcut handler'lardaki kullanım şeklini koru.
5. Specification: Ardalis.Specification, filtreleme/include mevcut spec'lerdeki gibi kurulur.
6. Paginate query'de `Common/Models/PaginationResponse.cs` kullan, yeni tip icat etme.
7. Domain event tetikleyen bir aksiyon varsa (Create/Update/Delete), event handler'ı da yaz —
   event'in nasıl eklendiğine dikkat et (entity ctor'unda mı, repository decorator mı ekliyor).

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
