# Backend Paket Haritası

Central Package Management (`Directory.Packages.props`) kullanılıyor — versiyon orada sabit,
proje dosyalarında version yazma.

| Paket                                                              | Kullanım Amacı                                                                                 |
| ------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------- |
| MediatR                                                            | CQRS — Command/Query/Notification (domain event) dispatch                                      |
| Ardalis.Specification (+ .EntityFrameworkCore)                     | Query specification pattern — `Specifications/` klasörleri                                     |
| FluentValidation (+ DI Extensions)                                 | Command/Query validator'ları                                                                   |
| Mapster                                                            | Entity ↔ DTO mapping (`.Adapt<T>()`)                                                           |
| Finbuckle.MultiTenant (+ AspNetCore, EFCore)                       | Multi-tenancy — database-per-tenant, `IsMultiTenant()`                                         |
| Npgsql.EntityFrameworkCore.PostgreSQL                              | EF Core PostgreSQL provider (ana DB — SqlServer paketi de var ama muhtemelen legacy/opsiyonel) |
| NewId                                                              | Sıralanabilir GUID üretimi (`BaseEntity` ctor'unda `NewId.Next().ToGuid()`)                    |
| Hangfire (+ Console, SqlServer/MySql/PostgreSql storage)           | Background job / scheduled task                                                                |
| ClosedXML                                                          | Excel export — `Export/` klasörlerindeki handler'lar                                           |
| MailKit                                                            | Email gönderimi                                                                                |
| Otp.Net                                                            | 2FA / TOTP                                                                                     |
| Serilog (+ sinks: Console, Seq, Elasticsearch, MSSqlServer, Async) | Loglama                                                                                        |
| NSwag.AspNetCore                                                   | OpenAPI/Swagger doc üretimi                                                                    |
| Microsoft.AspNetCore.Authentication.JwtBearer                      | JWT auth                                                                                       |
| Microsoft.AspNetCore.Identity(.EntityFrameworkCore)                | Kullanıcı/rol yönetimi (`ApplicationUser`)                                                     |
| Microsoft.AspNetCore.SignalR (+ StackExchangeRedis)                | Realtime (bildirim, canlı progress güncellemesi vb. için kullanılabilir)                       |
| Microsoft.Extensions.Caching.StackExchangeRedis                    | Redis cache                                                                                    |
| RazorEngineCore                                                    | Dinamik template render (email template gibi)                                                  |
| System.Linq.Dynamic.Core                                           | Dinamik filtreleme/sıralama (Paginate query'lerinde muhtemel kullanım)                         |
| Roslynator.Analyzers, StyleCop.Analyzers                           | Kod kalite analizi (derleme sırasında uyarı/hata üretir)                                       |

## Katman Sırası (gerçek klasör isimleri)

```
Core/Domain      → Entity, DomainEvent, Enum (hiçbir şeye bağımlı değil)
Core/Application → CQRS (MediatR), Validator, Specification, DTO, Interface tanımları
Core/Shared      → Cross-cutting concerns
Infrastructure   → EF Core Configuration, DbContext, Repository impl, dış servis entegrasyonları
PublicApi        → Endpoint/Controller, DI wiring, middleware ("Api" değil "PublicApi")
```

## Repository Pattern (rapor ile doğrulandı — önemli düzeltme)

Handler'lar muhtemelen `IRepository<T> : IRepositoryBase<T>` (generic repository) üzerinden
çalışıyor, `ApplicationDbRepository<T>` impl'i var. Ayrıca **`EventAddingRepositoryDecorator<T> :
IRepositoryWithEvents<T>`** — repository seviyesinde otomatik domain event ekleme (decorator pattern).
Bu, önceki varsayımımı ("handler'lar DbContext/DbSet kullanır") geçersiz kılıyor. Yeni handler
yazmadan önce mevcut bir handler'ın constructor'ına bakıp hangi soyutlamanın inject edildiğini
doğrula (bkz. SKILLS.md §0).

## Naming Belirsizliği (doğrulanacak)

`Products/Queries/Search/SearchProductsRequestHandler.cs` görüldü — `Request`/`RequestHandler`
naming, ekran görüntüsündeki `Lessons/Create/CreateLessonCommand.cs` (`Command`) naming'inden
farklı. İki feature farklı dönemde yazılmış olabilir. **Yeni kod `Lessons/` pattern'ini takip
etmeli** (screenshot'ta doğrulanmış, daha güncel görünüyor) ama emin olmak için Claude Code'un
ilk işi ikisini karşılaştırıp bana rapor etmesi (SKILLS.md §0).

## Multi-Tenancy Notu

`BaseDbContext.OnConfiguring` içinde her tenant'ın kendi `ConnectionString`'i ile bağlanılıyor
(database-per-tenant). `IsMultiTenant()` çağrısı bu senaryoda zorunlu izolasyon aracı değil,
ama **Course zincirinde tutarlılık için kullanılmaya devam ediliyor**. Yeni entity eklerken
hangi grup içine girdiğine göre karar ver (bkz. `CLAUDE.md` madde 4).
