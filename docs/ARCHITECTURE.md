# Mimari, Proje Yapısı ve Özellik Detayları

[← Ana Sayfaya Dön (README.md)](../README.md)

Bu doküman, **Serie A Futbol Ligi Yönetim ve İstatistik Platformu**'nun klasör yapısını, mimari kararlarını ve uygulama ekranlarının detaylı iş kurallarını açıklar.

## İçindekiler

1. [Proje yapısı](#proje-yapısı)
2. [Teknik özellikler ve mimari kararlar](#teknik-özellikler-ve-mimari-kararlar)
3. [Uygulama modülleri ve ekranlar](#uygulama-modülleri-ve-ekranlar)

---

## Proje yapısı

```text
SerieA.sln
├── docs/                               # Proje Dokümantasyonu ve Ekran Görüntüleri
│   ├── images/                         # Ziyaretçi portalı ve yönetim paneli ekran görüntüleri
│   ├── ARCHITECTURE.md                 # Mimari, proje yapısı ve özellik detayları
│   └── API-ENDPOINTS.md                # REST API uç noktaları ve veritabanı içeriği
│
├── SerieA.API/                         # RESTful Web API Katmanı (.NET 8.0)
│   ├── Common/                         # ApiResponse<T>, ApiResponseExtensions, JwtOptions
│   ├── Context/                        # ApiContext (EF Core DbContext ve Fluent API ayarları)
│   ├── Controllers/                    # Auth, Dashboard, Matches, MatchEvent, Standings, Teams, Users
│   ├── Data/
│   │   ├── seriea-data.sql             # Tablo şemaları + hazır gerçek Serie A verilerini içeren SQL betiği
│   │   └── team-rosters.json           # Serie A takımlarının oyuncu kadroları
│   ├── DTOs/                           # Veri Transfer Nesneleri (Auth, Dashboard, Matches, Standings, Teams, Users)
│   ├── Entities/                       # Veritabanı Varlıkları (Team, Match, MatchGoal, MatchCard, Substitution, User)
│   ├── Enums/                          # CardType, MatchStatus, UserRole
│   ├── Mapping/                        # AutoMapper Profilleri
│   ├── Middlewares/                    # Global ExceptionMiddleware
│   ├── Migrations/                     # EF Core Code-First Migration dosyaları
│   ├── Services/                       # İş Katmanı Servisleri ve Arayüzleri (Interfaces)
│   └── Validators/                     # FluentValidation doğrulama sınıfları
│
└── SerieA.WebUI/                       # ASP.NET Core MVC Kullanıcı ve Yönetim Arayüzü (.NET 8.0)
    ├── Controllers/                    # Public, Home, Teams, Matches, Weeks, Standings, Users, Account
    ├── DTOs/ & ViewModels/             # API iletişim DTO'ları ve Razor View modelleri
    ├── Extensions/                     # ServiceExtensions (Typed HttpClient), MappingExtensions, JwtHelper
    ├── Handlers/                       # AuthHeaderHandler (Bearer Token), ApiExceptionHandler
    ├── Services/                       # API ile haberleşen HttpClient servisleri (*ApiService)
    ├── Views/                          # Razor (.cshtml) görünümleri, Partial View'lar ve Modallar
    └── wwwroot/                        # Özel CSS, JavaScript (AJAX modülleri), logolar ve kadro JSON verisi
```

---

## Teknik özellikler ve mimari kararlar

**İlişkisel veritabanı ve EF Core Code-First (`ApiContext`).** `Team` ile `Match` arasındaki `HomeTeam` ve `AwayTeam` ilişkilerinde, döngüsel silme (multiple cascade paths) hatasını önlemek için `DeleteBehavior.Restrict` kullanılır. `Match` ile `MatchGoal`, `MatchCard` ve `Substitution` arasındaki ilişkilerde `DeleteBehavior.Cascade` kullanılır; maç silindiğinde alt olaylar da temizlenir.

**Standart API yanıt zarfı (`ApiResponse<T>`).** Tüm uç noktalar `IsSuccess`, `Message`, `Data` ve `Errors` alanlarını içeren tek tip bir JSON formatıyla yanıt verir.

**Merkezi hata yönetimi.**
- API tarafında `ExceptionMiddleware`, beklenmeyen hataları yakalar ve anlamlı HTTP durum kodlarına (`400`, `401`, `404`, `500`) çevirir.
- WebUI tarafında `ApiExceptionHandler` (`DelegatingHandler`), FluentValidation hataları dahil tüm hata yanıtlarını yakalar. Arayüzün çökmesini engeller ve hataları kullanıcı dostu mesajlara dönüştürür.

**JWT ve cookie köprüsü (`AuthHeaderHandler` ve `JwtHelper`).**
- Kullanıcı WebUI üzerinden giriş yaptığında API'den alınan JWT, `HttpOnly` kimlik doğrulama çerezinin (`.SerieA.Auth`) içine gömülür.
- WebUI'dan API'ye giden her HTTP isteğine `AuthHeaderHandler` otomatik olarak `Authorization: Bearer <token>` başlığını ekler.
- Her sayfa isteğinde `OnValidatePrincipal` olayı ve `JwtHelper.IsTokenExpired` ile token süresi kontrol edilir. Süresi dolmuşsa kullanıcı oturumdan otomatik olarak çıkarılır.

**CSRF koruması.** Tüm MVC POST isteklerinde `AutoValidateAntiforgeryTokenAttribute`, AJAX isteklerinde `X-CSRF-TOKEN` başlığı zorunludur.

---

## Uygulama modülleri ve ekranlar

### Ziyaretçi portalı

Bu ekranlara oturum açmadan erişilir.

#### Puan durumu

Rotalar: `/`, `/Puan-Durumu`, `/Public/Standings`

- Lig tablosu tamamlanan maçlardan dinamik olarak hesaplanır. Tabloda oynanan maç, galibiyet, beraberlik, mağlubiyet, atılan gol, yenilen gol, averaj, puan ve son 5 maçın form göstergesi (`W / D / L`) yer alır.
- `upToWeek` parametresiyle ligin geçmişteki herhangi bir haftası itibarıyla puan durumu anlık olarak hesaplanır.
- Şampiyonlar Ligi, Avrupa Ligi, Konferans Ligi ve küme düşme hatları renk göstergeleriyle işaretlenir.

#### Haftalık fikstür

Rotalar: `/Fikstur`, `/Public/Fixtures`

- Etkileşimli hafta seçici ile 1. haftadan 38. haftaya kadar geçiş yapılabilir.
- Maçlar durumlarına göre (`Başlamadı`, `Canlı`, `Tamamlandı`, `Ertelendi`) filtrelenir ve rozetlerle gösterilir.

#### Maç merkezi

Rotalar: `/Mac/{id}`, `/Public/MatchDetail/{id}`

- Maç skoru, stadyum bilgisi, tarih ve saat ile kulüp logoları gösterilir.
- Goller, sarı/kırmızı kartlar ve oyuncu değişiklikleri kronolojik bir zaman çizelgesinde listelenir. Uzatma dakikaları (`45+2`, `90+4` gibi) desteklenir.

---

### Yönetim paneli (Admin ve Editor)

Yönetim ekranları JWT + Cookie tabanlı kimlik doğrulamayla korunur.

#### Genel bakış

Rotalar: `/Admin`, `/Home/Index`

- Aktif takım sayısı, tamamlanan ve toplam maç sayısı, toplam gol, maç başına gol ortalaması ve sıradaki hafta gösterilir.
- Son tamamlanan 6 maç, yaklaşan 5 maç ve güncel puan durumundaki ilk 5 takım hızlı özet olarak sunulur.

#### Takım yönetimi

Rota: `/Teams`

- Kulüpler listelenir; bölgeye, duruma veya arama kelimesine göre filtrelenir.
- Lig özeti şeridi (`LeagueSummary`) ortak kullanılan stadyumları (örneğin San Siro / Giuseppe Meazza) tekilleştirir. Toplam stadyum kapasitesini, temsil edilen şehir sayısını ve ligin en köklü kulübünü otomatik hesaplar.
- Serie A kuralları gereği en fazla 20 aktif takım sınırı denetlenir.
- Kulüp silindiğinde geçmiş maç verileri bozulmasın diye soft delete uygulanır (`Status = false`).

#### Maç ve fikstür yönetimi

Rotalar: `/Matches`, `/Weeks`

- Maçlar hafta ve takım bazında filtrelenir.
- AJAX tabanlı modallar sayfayı yenilemeden yeni maç eklemeye, skor ve durum güncellemeye, maç silmeye izin verir.
- Aynı takımın hem ev sahibi hem deplasman olarak seçilmesi doğrulama kuralıyla engellenir.

#### Maç içi olay yönetimi

Rota: `/Matches/Detail/{id}`

- Maça gol, sarı/kırmızı kart ve oyuncu değişikliği (giren/çıkan oyuncu) eklenebilir ve silinebilir.
- `team-rosters.json` entegrasyonu sayesinde seçilen takımın gerçek Serie A kadrosu otomatik yüklenir.
- Skor uyuşmazlığı denetimi (`HasScoreMismatch`), maç skoruyla girilen gol kayıtlarının sayısı tutmadığında yöneticiyi uyarır.

#### Kullanıcı ve yetki yönetimi

Rota: `/Users` (yalnızca `Admin` rolü)

- Yeni `Admin` veya `Editor` kullanıcısı eklenebilir; kullanıcı bilgileri ve şifresi güncellenebilir; hesap aktif/pasif yapılabilir ve silinebilir.
- Şifreler veritabanında açık metin olarak tutulmaz. `IPasswordHasher<User>` (PBKDF2) ile hashlenerek saklanır.

