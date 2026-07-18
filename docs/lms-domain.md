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

## Bilinen Sistemik Sorunlar
- **FluentValidation pipeline bağlı değil** — validator'lar DI'da kayıtlı ama MediatR pipeline'ına
  bağlayan bir `IPipelineBehavior` yok, bu yüzden hiçbir validator gerçekte çalışmıyor (Task 12'de
  detaylandırıldı, henüz çözülmedi). Kritik kontroller şimdilik handler içinde manuel yapılıyor
  (örn. `EnrollInCourseRequestHandler`'daki duplicate-enrollment kontrolü).
- **`AuditableEntity.CreatedOn` get-only** olduğu için EF Core tarafından map'lenmiyor —
  **hiçbir tabloda persist edilmiyor**. `CreatedOn`'a göre sıralama/filtreleme yapan başka bir
  yer varsa aynı 500 riskini taşır (`NotesByLessonPageSpec`'te bulunup `OrderBy(Id)` ile
  atlatıldı — UUIDv7 id'ler zaman sıralı olduğu için işlevsel eşdeğer, ama kalıcı çözüm değil).
- **`AuditableEntity.CreatedOn` gerçek anlamda kullanılabilir hale getirilmeli** — backing
  field + EF mapping + migration ile her tabloya gerçek `CreatedOn` kolonu eklenmeli. Şu an
  `LastModifiedOn`'un create anındaki değeri `CreatedOn` yerine kullanılıyor (Notes'ta:
  `MapsterSettings` içinde `NoteDto.CreatedOn ← Note.LastModifiedOn` mapping'i), bu geçici
  bir çözüm.
- **Quiz doğru cevap bilgisi backend'de yapısal değil** — `LessonPage.Content` düz HTML,
  `(richtig)` işaretine dayalı geçici DOMParser var (`QuizContent.vue`). Gerçek çözüm:
  LessonPage'e ayrı bir Quiz veri modeli (Question/Option/IsCorrect) eklenmeli — ayrı,
  kapsamlı bir task.
- **Deep-link `/lessons/:id/player` sayfası (blank layout) kayıtlı dark temayı uygulamıyor**
  çünkü ThemeSwitcher sadece admin layout'ta mount oluyor. Modal (v-dialog) zaten ana giriş
  yolu olduğu için düşük öncelikli, ama app-seviyesi tema mount sorunu genel olarak var.
- **Domain Event çifte tetiklenmesi** (Task 1'de bulunmuştu, PPTX import Task B'de tekrar
  doğrulandı): `EventAddingRepositoryDecorator` her `AddAsync()`'te otomatik event ekliyor,
  ama `CreateLessonRequestHandler`/`CreateChapterRequestHandler` gibi handler'lar bunu manuel
  olarak da yapıyor — event'ler iki kez tetikleniyor. Şu anki event handler'lar (log-only)
  ucuz olduğu için performans etkisi düşük, ama gerçek side-effect'li bir event handler
  eklenirse (örn. email gönderme) bug haline gelir. Kapsamlı düzeltme: tüm Create
  handler'lardan manuel event ekleme satırlarını kaldırmak — ayrı, orta ölçekli bir task.
- **`PptxLessonExtractor` bullet-listesi tespiti implicit slide-layout/master mirasını
  yakalamıyor**, sadece slide XML'inde açıkça tanımlı bullet'ları yakalıyor — çoğu gerçek
  PPTX dosyasında bullet'lar layout'tan miras alındığı için düz paragraf olarak gelebilir.
  Kozmetik, düşük öncelik.

## Design Backlog

- **Quiz şu an her seçenek tıklandığında anında doğru/yanlış gösteriyor** (immediate feedback).
  Orijinal tasarım referansı "Zurück / Auswertung ansehen" butonlarıyla tüm sorular
  cevaplandıktan sonra toplu değerlendirme modeliydi. Karar: şimdilik mevcut anlık model
  korunuyor, ileride orijinal modele dönülüp dönülmeyeceği ayrıca değerlendirilecek.
