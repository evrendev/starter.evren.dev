# EvrenDev — Proje Bağlamı

Multi-tenant LMS (Learning Management System). GitLab University tarzı, slide/sayfa bazlı ders
oynatıcı hedefleniyor. **Bu dosya kısa tutulur — detay gerektiğinde `docs/` altındaki ilgili dosyayı oku, hepsini önden okuma.**

## Stack (özet)

| Katman        | Teknoloji                                                       |
| ------------- | --------------------------------------------------------------- |
| Backend       | .NET 9, PostgreSQL (Npgsql), Clean Architecture, MediatR (CQRS) |
| Multi-tenancy | Finbuckle.MultiTenant — **database-per-tenant**                 |
| Frontend      | Vue 3 + TypeScript, Vuetify 3, Pinia, Axios, vue-router         |

Paket detayları ve "ne ne için kullanılıyor" → `docs/backend-stack.md`, `docs/frontend-stack.md`.

## Klasör Yapısı (gerçek, doğrulandı)

```
src/backend/
├── Core/
│   ├── Domain/         # Entity, DomainEvent, Enum — hiçbir şeye bağımlı değil
│   ├── Application/     # CQRS, Validator, Specification, DTO, Interface
│   └── Shared/          # Cross-cutting concerns
├── Infrastructure/       # EF Core Configuration, DbContext, Repository impl
└── PublicApi/            # Endpoint/Controller (dikkat: "Api" değil "PublicApi")
```

## Mimari Kurallar (her zaman geçerli)

1. **Katman sırası**: Core/Domain → Core/Application → Infrastructure → PublicApi.
2. **Entity pattern**: `AuditableEntity`/`IAggregateRoot` + private setter + constructor + `Update()` metodu.
   Yeni entity yazarken mevcut bir entity'yi (örn. `Course.cs`) şablon al, icat etme.
3. **Repository Pattern** — handler'lar doğrudan `DbContext` DEĞİL, `IRepository<T>` inject ediyor.
   - Write operations (Create/Update/Delete): `IRepository<T>` inject
   - Read operations (Get/Paginate/Export/Search): `IReadRepository<T>` veya `IRepository<T>` inject
   - Domain events otomatik: `EventAddingRepositoryDecorator<T>` DI'da registered, Add/Update/Delete
     çağrılarında event injection yapılıyor (handler'da manuel ekleme yapma, decorator bunu yapar).
4. **Application klasör deseni** — kesin doğrulanmış:
   ```
   Lessons/ (ve tüm features)
   ├── Entities/           LessonDto, LessonDetailsDto, LessonExportDto
   ├── EventHandlers/      LessonCreatedEventHandler, LessonUpdatedEventHandler, LessonDeletedEventHandler
   ├── Queries/
   │   ├── Create/         CreateLessonRequest + Handler + Validator
   │   ├── Update/         UpdateLessonRequest + Handler + Validator
   │   ├── Delete/         DeleteLessonRequest + Handler
   │   ├── Get/            GetLessonRequest + Handler (GetAllLessonsRequest + Handler)
   │   ├── Paginate/       PaginateLessonsFilter + Handler
   │   ├── Export/         ExportLessonsRequest + Handler
   │   └── Search/         (Products'de var, Lessons'da yok ama pattern aynı)
   └── Specifications/     Ardalis.Specification<TEntity, TProjection> türü
   ```
   - **Naming**: `{Action}LessonRequest` (Command/Query değil, her zaman Request)
   - **Handler**: `{Action}LessonRequestHandler` (aynı dosyada Request ve Validator ile)
5. **Multi-tenancy**: `IsMultiTenant()` sadece `Category→Course→Chapter→Lesson→LessonPage` zincirinde
   çağrılır. Composite-key ilişki tabloları (`CourseEnrollment`, `LessonProgress`, `LessonPageProgress`)
   bilinçli olarak `IsMultiTenant()` almaz (zaten tenant-scoped DB + FK zinciri üzerinden izole).
6. **Domain Events**: `SaveChangesAsync` sonrası `IEntity.DomainEvents` otomatik yayınlanır (MediatR
   notification) — bkz. `BaseDbContext`. Repository decorator ile de event eklenebiliyor olabilir (madde 3).
   Rollup/side-effect mantığı (progress hesaplama vb.) event handler'da yazılır, command içine gömülmez.
7. **LMS domain modeli** (Category/Course/Chapter/Lesson/LessonPage/Progress hiyerarşisi ve karar
   gerekçeleri) → `docs/lms-domain.md`. Yeni LMS özelliği eklerken önce bunu oku.
8. **Pagination**: `Common/Models/PaginationResponse.cs` mevcut — yeni "Paginate" query yazarken bu
   sınıfı kullan, kendi pagination response tipini icat etme.

## Çalışma Kuralları (token verimliliği)

- Kod yazmadan önce **ilgili mevcut dosyayı oku**, kendi konvansiyonunu icat etme.
- Yıkıcı işlemler (migration çalıştırma, kolon drop, `dotnet ef database update`) öncesi **dur, onay iste**.
- Gereksiz dosya taraması yapma; `find`/`grep` ile hedefli ara, tüm repo'yu okuma.
- Değişiklik sonunda sadece **değişen dosya listesini** özetle, kod tekrar basma.

## Referans Dosyalar

- `docs/backend-stack.md` — backend paket haritası
- `docs/frontend-stack.md` — frontend paket haritası (reveal.js, quill vs. nerede kullanılacak)
- `docs/lms-domain.md` — LMS entity modeli ve progress event zinciri
- `SKILLS.md` — senaryo bazlı checklist (yeni entity, yeni CQRS feature, yeni Vue sayfası...)
