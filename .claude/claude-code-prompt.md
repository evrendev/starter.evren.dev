# Claude Code Görev Sırası — LessonPage (Slide) Altyapısı

Her görevi AYRI bir mesaj olarak Claude Code'a yapıştır. Bir görev bitip commit atılmadan
sıradakine geçme — her görev bir öncekinin çıktısına güvenir.

---

## Task 0 — Bağlam Dosyalarını Yerleştir + Commit

```
CLAUDE.md, SKILLS.md ve docs/ klasörünü bu repoya ekle (zaten kopyaladıysam sadece oku ve
içeriğini onayla). Değişiklik yoksa hiçbir şey yapma. Sonra şu commit'i at:

git add CLAUDE.md SKILLS.md docs/
git commit -m "docs: proje bağlamı ve mimari playbook eklendi (CLAUDE.md, SKILLS.md, docs/)"
```

---

## Task 1 — Konvansiyon Doğrulama (kod yazma, sadece keşif + rapor)

```
CLAUDE.md ve SKILLS.md'yi oku (SKILLS.md §0'daki doğrulama listesine özellikle bak).

Şunları keşfet ve bana özetle — HENÜZ KOD YAZMA:

1. `Core/Application/Catalog/Lessons/` altındaki TÜM dosyaları listele ve her klasörden
   (Entities, EventHandlers, Create, Update, Delete, Get, Paginate, Export, Specifications)
   birer dosyanın içeriğini oku.
2. `Core/Application/Catalog/Products/` altındaki yapıyı da listele (Queries/Search/ var mıydı,
   Command mi Request mi kullanılıyor).
3. Bir Lessons handler'ının constructor'ını incele: `IRepository<T>` / `IReadRepositoryBase<T>`
   mi inject ediliyor, yoksa `ApplicationDbContext` mi? `EventAddingRepositoryDecorator<T>`
   nerede tanımlı, nasıl DI'a bağlanıyor (bul: `grep -rn "EventAddingRepositoryDecorator" .`)?
4. `Common/Models/PaginationResponse.cs` içeriğini oku.
5. Bir Specification örneğini (Lessons ya da Chapters altında) oku.
6. Domain event'in tam akışını doğrula: entity ctor'unda mı `DomainEvents.Add(...)` yapılıyor,
   repository decorator mı ekliyor, yoksa ikisi de mi?

Bulgularını CLAUDE.md §3-4'teki "DOĞRULANACAK" notlarını GÜNCELLEYECEK şekilde bana raporla.
Eğer Lessons ile Products arasında gerçekten pattern farkı varsa, hangisinin yeni kod için
"kanonik" kabul edileceğine dair önerini de belirt (varsayımım: Lessons, çünkü ekran
görüntüsünde doğrulanmıştı). Onayımı bekle, sonra CLAUDE.md/SKILLS.md'deki "DOĞRULANACAK"
notlarını gerçek bulgularla güncelle ve şu commit'i at:

git add CLAUDE.md SKILLS.md
git commit -m "docs: gercek kod konvansiyonlarina gore CLAUDE.md/SKILLS.md guncellendi"
```

---

## Task 2 — Domain Katmanı

```
docs/lms-domain.md'yi oku. Aşağıdaki Domain değişikliklerini yap (mevcut Course.cs/Lesson.cs
pattern'ini birebir takip et, Task 1'de doğrulanan domain event ekleme yöntemini kullan):

1. Yeni: LessonPage.cs (Title, Content, ContentType enum [Text/Video/Image/Quiz/Embed], Order,
   MediaUrl, LessonId, Lesson nav, Notes koleksiyonu, Progress koleksiyonu)
2. Yeni: LessonPageProgress.cs (UserId, LessonPageId, Completed, CompletedAt, LastVisitedAt)
3. Lesson.cs: Content kaldır, Order ekle, Pages koleksiyonu ekle, Notes koleksiyonunu kaldır
4. Chapter.cs: Order ekle
5. Note.cs: LessonId yerine LessonPageId kullan
6. LessonProgress.cs: Completed(bool) yerine ProgressStatus enum + PercentComplete +
   LastVisitedPageId + CompletedAt

Değiştirdiğin/oluşturduğun dosyaları listele, `dotnet build` çalıştırıp Domain projesinin
hatasız derlendiğini doğrula (henüz Infrastructure/Application'da referans hataları normal,
sadece Domain projesini derle). Sonra commit at:

git add -A
git commit -m "feat(domain): LessonPage ve LessonPageProgress eklendi; Lesson/Chapter/Note/LessonProgress guncellendi"
```

---

## Task 3 — Infrastructure Katmanı

```
Task 2'deki Domain değişikliklerine göre:

1. Yeni: LessonPageConfig.cs (LessonConfig.cs'i şablon al, IsMultiTenant() ekle)
2. Yeni: LessonPageProgressConfig.cs (LessonProgressConfig.cs'i şablon al, composite key
   UserId+LessonPageId)
3. LessonConfig.cs güncelle: Pages ilişkisini kur
4. ChapterConfig.cs: Order için ekstra config gerekmiyor, kontrol et
5. NoteConfig.cs güncelle: FK'yi LessonPage'e bağla
6. ApplicationDbContext.cs'e DbSet<LessonPage> ve DbSet<LessonPageProgress> ekle

`dotnet build` ile Infrastructure projesinin hatasız derlendiğini doğrula. Commit at:

git add -A
git commit -m "feat(infrastructure): LessonPage EF Core configuration ve DbContext kayitlari"
```

---

## Task 4 — Application Katmanı: LessonPage CRUD

```
SKILLS.md §1-2'deki checklist'i ve Task 1'de doğrulanan gerçek pattern'i (Command mi Request mi,
IRepository<T> kullanımı) uygulayarak `Core/Application/Catalog/LessonPages/` altında Lessons
feature'ını şablon alan tam bir CRUD seti oluştur:

Entities/ (LessonPageDto, LessonPageDetailsDto, LessonPageExportDto)
EventHandlers/ (Created/Updated/Deleted)
Create/, Update/, Delete/, Get/, Paginate/ (PaginationResponse kullan), Export/, Specifications/

Her command/query alanları: Title, Content, ContentType, Order, LessonId, MediaUrl (+ Id where
relevant). Mevcut Lessons validator/handler stilini birebir kopyala.

Oluşturduğun dosyaların listesini ver, `dotnet build` ile Application projesini derle. Commit at:

git add -A
git commit -m "feat(application): LessonPage CQRS CRUD feature seti"
```

---

## Task 5 — Player Query + Progress Rollup Event Zinciri

```
docs/lms-domain.md'deki progress zincirini uygula:

1. GetLessonPlayerQuery: Input LessonId (+ current user). Output: Lesson başlığı + Order'a göre
   sıralı tüm LessonPage'ler + her sayfanın LessonPageProgress durumu + genel yüzde + LastVisitedPageId.
2. MarkLessonPageCompletedCommand: LessonPageProgress upsert eder, LessonPageCompletedEvent fırlatır.
3. RecalculateLessonProgressOnPageCompletedHandler (notification handler): LessonPageCompletedEvent'i
   dinler, o Lesson'daki tamamlanma oranını hesaplayıp LessonProgress günceller; %100 ise
   LessonCompletedEvent fırlatır.
4. RecalculateCourseProgressOnLessonCompletedHandler: LessonCompletedEvent'i dinler, CourseEnrollment
   ilerlemesini günceller.

Mevcut IEventPublisher/MediatR notification altyapısını kullan, yeni mekanizma kurma.
`dotnet build` ile derle, commit at:

git add -A
git commit -m "feat(application): lesson player query ve 3 seviyeli progress rollup event zinciri"
```

---

## Task 6 — PublicApi Endpoint'leri

```
Lessons feature'ının PublicApi'daki route tanımını (controller mı, minimal API grubu mu, versioning
attribute'ları, NSwag doc) şablon alarak LessonPage için endpoint'leri oluştur:
POST /lessons/{lessonId}/pages, PUT /lesson-pages/{id}, DELETE /lesson-pages/{id},
GET /lesson-pages/{id}, GET /lessons/{lessonId}/pages, GET /lessons/{lessonId}/player,
POST /lesson-pages/{id}/complete

`dotnet build` ile TÜM solution'ı derle (artık her katman birbirine bağlı, tam build gerekli).
Hata varsa raporla, düzelt. Commit at:

git add -A
git commit -m "feat(api): LessonPage endpoint'leri (player ve complete dahil)"
```

---

## Task 7 — Migration (ONAY GEREKTİRİR, ÇALIŞTIRMA)

```
`dotnet ef migrations add AddLessonPageSupport` komutunu (repo'daki gerçek proje/startup
parametreleriyle, README'de nasıl geçiyorsa) çalıştırarak migration dosyasını OLUŞTUR ama
`dotnet ef database update` ÇALIŞTIRMA. Migration dosyasının içeriğini bana göster — özellikle
Lesson.Content kolonunun drop edilip edilmediğine, eski veri varsa kaybolup kaybolmayacağına
dikkat et. Onayımı bekle. Onay sonrası:

git add -A
git commit -m "chore(db): AddLessonPageSupport migration"
```
