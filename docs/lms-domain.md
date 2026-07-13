# LMS Domain Modeli (GitLab University tarzı, slide bazlı)

## Hiyerarşi
```
Category
 └─ Course
     └─ Chapter (Order)
         └─ Lesson (Order)             ← Content YOK, sadece container
             └─ LessonPage (Order)      ← asıl "slide", Content burada
```

## LessonPage
```csharp
Title, Content, ContentType (Text/Video/Image/Quiz/Embed enum), Order, MediaUrl, LessonId
```
`Content` düz Markdown/HTML string. Blok-tabanlı (JSONB) yapı bilinçli olarak ERTELENDİ —
GitLab University sayfaları sade, MVP'de gerek yok.

## Progress Zinciri (3 seviye rollup)
```
LessonPageProgress (UserId, LessonPageId, Completed, CompletedAt, LastVisitedAt)
   ↓ LessonPageCompletedEvent
LessonProgress (UserId, LessonId, Status: NotStarted/InProgress/Completed, PercentComplete, LastVisitedPageId)
   ↓ LessonCompletedEvent
CourseEnrollment (UserId, CourseId, PercentComplete)   ← dashboard'daki "%1 Complete" avatarı
```
Rollup hesaplaması **command handler içinde değil**, domain event'i dinleyen ayrı notification
handler'larda yapılır (bkz. SKILLS.md #3). Mevcut `IEventPublisher` / `SendDomainEventsAsync`
altyapısı (BaseDbContext) buna hazır, yeni bir mekanizma kurmaya gerek yok.

## Note
`Note.LessonPageId` — Lesson'a değil, **sayfaya** bağlı (GitLab'da not o an bakılan slide'a aittir).
`Lesson`'a erişim gerekirse `Note.LessonPage.Lesson` üzerinden.

## Multi-Tenancy
`Category, Course, Chapter, Lesson, LessonPage` → `IsMultiTenant()`.
`CourseEnrollment, LessonProgress, LessonPageProgress` → `IsMultiTenant()` YOK (composite key
ilişki tablosu, mevcut konvansiyon böyle — bkz. `docs/backend-stack.md`).

## Repository Kullanımı — Composite-Key Entity Uyarısı
`CourseEnrollment, LessonProgress, LessonPageProgress` composite-key entity'lerdir —
`IRepository<T>.GetByIdAsync(id)` ile ÇAĞRILMAMALI (gerçek tekil Guid Id'leri yok, runtime
hatası verir). Bunun yerine `FirstOrDefaultAsync(spec)` veya `CountAsync(spec)` kullanılmalı.

## Henüz Kod Yazılmadı
Bu dosya sadece **tasarım kararlarının kaydı**. Uygulama adımları için
`claude-code-prompt-lessonpage.md` promptunu kullan (Adım 0 keşfi zorunlu — mevcut `Lesson`
feature'ının Result tipi, validator stili, specification pattern'i önce doğrulanmalı).

## Açık Kararlar
- Frontend player: reveal.js mi, Vuetify custom mu? → `docs/frontend-stack.md`
- `Lesson.Content` alanı kaldırılınca eski veri migration'da nasıl taşınacak? (Adım 5'te ele alınacak)
