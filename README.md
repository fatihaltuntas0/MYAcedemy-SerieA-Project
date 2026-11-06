# Serie A Futbol Ligi Yönetim ve İstatistik Platformu

Serie A Platformu, İtalya Serie A Futbol Ligi'ndeki takımları, haftalık fikstürleri, canlı ve tamamlanmış maç sonuçlarını, maç içi olayları (goller, sarı/kırmızı kartlar, oyuncu değişiklikleri) ve dinamik puan durumunu yönetmek ve göstermek için **.NET 8.0** ile geliştirilmiş bir full-stack web uygulamasıdır.

Proje birbirinden ayrık iki katmandan oluşur: RESTful Web API (`SerieA.API`) ve bu API'yi tüketen ASP.NET Core MVC web arayüzü (`SerieA.WebUI`). Uygulamanın iki yüzü vardır: futbolseverlere açık bir public istatistik ve fikstür portalı ile yetkili kullanıcılar (Admin ve Editor) için bir yönetim paneli.

## İçindekiler

1. [Özellikler](#özellikler)
2. [Teknik özellikler](#teknik-özellikler)
3. [Proje yapısı](#proje-yapısı)
4. [Teknolojiler ve paket sürümleri](#teknolojiler-ve-paket-sürümleri)
5. [Kurulum](#kurulum)
6. [Veritabanı içeriği ve giriş bilgileri](#veritabanı-içeriği-ve-giriş-bilgileri)
7. [API uç noktaları](#api-uç-noktaları)

## Özellikler

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

## Teknik özellikler

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

## Proje yapısı

```text
SerieA.sln
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

## Teknolojiler ve paket sürümleri

Aşağıdaki sürümler, projeyi derlemek veya paketleri sıfırdan kurmak için gereken tam sürümlerdir.

| Bileşen | Değer |
| :--- | :--- |
| Hedef framework | .NET 8.0 (`net8.0`) |
| Veritabanı | Microsoft SQL Server (Express, Developer veya LocalDB) |

### SerieA.API paketleri

| Paket | Sürüm | Kullanım amacı |
| :--- | :---: | :--- |
| `Microsoft.EntityFrameworkCore` | `8.0.32` | ORM çekirdek kütüphanesi |
| `Microsoft.EntityFrameworkCore.SqlServer` | `8.0.32` | SQL Server veritabanı sağlayıcısı |
| `Microsoft.EntityFrameworkCore.Tools` | `8.0.32` | Migration ve veritabanı güncelleme araçları (`Add-Migration`, `Update-Database`) |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `8.0.33` | JWT tabanlı kimlik doğrulama |
| `AutoMapper` | `12.0.1` | Entity ve DTO nesneleri arasında dönüşüm |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | `12.0.1` | AutoMapper'ın .NET DI konteynerine kaydı |
| `FluentValidation.AspNetCore` | `11.3.1` | Gelen DTO modellerinin kurallara göre doğrulanması |
| `Swashbuckle.AspNetCore` | `6.6.2` | Swagger / OpenAPI dokümantasyonu ve test arayüzü |

Paketleri CLI ile tek tek kurmak isterseniz:

```powershell
dotnet add SerieA.API/SerieA.API.csproj package AutoMapper --version 12.0.1
dotnet add SerieA.API/SerieA.API.csproj package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add SerieA.API/SerieA.API.csproj package FluentValidation.AspNetCore --version 11.3.1
dotnet add SerieA.API/SerieA.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.33
dotnet add SerieA.API/SerieA.API.csproj package Microsoft.EntityFrameworkCore --version 8.0.32
dotnet add SerieA.API/SerieA.API.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.32
dotnet add SerieA.API/SerieA.API.csproj package Microsoft.EntityFrameworkCore.Tools --version 8.0.32
dotnet add SerieA.API/SerieA.API.csproj package Swashbuckle.AspNetCore --version 6.6.2
```

### SerieA.WebUI paketleri

| Paket | Sürüm | Kullanım amacı |
| :--- | :---: | :--- |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | `8.0.23` | MVC Controller ve View şablon üretimi (scaffolding) |

```powershell
dotnet add SerieA.WebUI/SerieA.WebUI.csproj package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 8.0.23
```

Repoyu klonladığınızda paket referansları `.csproj` dosyalarında hazır tanımlıdır. `dotnet restore` çalıştırmanız, tüm paketleri doğru sürümleriyle indirmek için yeterlidir.

## Kurulum

### Ön gereksinimler

1. [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --version` ile kontrol edebilirsiniz)
2. Microsoft SQL Server (Express, Developer veya MSSQLSERVER) ve SQL Server Management Studio (SSMS)
3. Visual Studio 2022 (önerilir), VS Code veya JetBrains Rider

### 1. Repoyu klonlama ve paketleri yükleme

```powershell
git clone <repo-url>
cd SerieA.API
dotnet restore
```

### 2. Veritabanını oluşturma

Projede tabloları ve gerçek Serie A verilerini içeren hazır bir SQL betiği bulunur:

`SerieA.API/Data/seriea-data.sql`

Betik 20 Serie A kulübü, 40 maç, 92 gol, 129 sarı/kırmızı kart, 247 oyuncu değişikliği ve hazır bir Admin kullanıcısı yükler.

#### Yöntem A: Hazır SQL dosyasıyla kurulum (önerilen)

1. SSMS'i açın ve SQL Server örneğinize bağlanın (örneğin `.\SQLEXPRESS` veya `.`).
2. `seriea-data.sql` dosyası `USE [SerieAProject]` komutuyla başladığı için önce boş veritabanını oluşturun. **New Query** ekranında şu komutu çalıştırın:
   ```sql
   CREATE DATABASE [SerieAProject];
   GO
   ```
3. `File -> Open -> File...` menüsünden `SerieA.API/Data/seriea-data.sql` dosyasını açın ve **Execute (F5)** ile çalıştırın.
4. İşlem bittiğinde `Teams`, `Matches`, `MatchGoals`, `MatchCards`, `Substitutions`, `Users` ve `__EFMigrationsHistory` tabloları örnek verilerle birlikte oluşur.

#### Yöntem B: EF Core Migrations ile boş şema kurulumu

Hazır verileri kullanmadan yalnızca boş veritabanı şemasını oluşturmak isterseniz:

```powershell
dotnet ef database update --project SerieA.API
```

Visual Studio Package Manager Console kullanıyorsanız Default Project olarak `SerieA.API` seçip `Update-Database` komutunu çalıştırabilirsiniz.

### 3. Yapılandırma

#### Veritabanı bağlantı cümlesi

`SerieA.API/appsettings.json` içindeki `ConnectionStrings:Default` alanını kendi SQL Server sunucu adınıza göre kontrol edin. Varsayılan değer `.\SQLEXPRESS`'tir:

```json
"ConnectionStrings": {
  "Default": "Server=.\\SQLEXPRESS;Database=SerieAProject;integrated security=true;trustServerCertificate=true"
}
```

Yerel varsayılan SQL Server örneği kullanıyorsanız `Server=.;`, LocalDB kullanıyorsanız `Server=(localdb)\\MSSQLLocalDB;` yazabilirsiniz.

#### JWT güvenlik anahtarı (`Jwt:SecretKey`)

> [!IMPORTANT]
> Güvenlik nedeniyle `SerieA.API/appsettings.json` içindeki `Jwt:SecretKey` alanı boştur; geliştirme ortamında .NET User Secrets kullanılmıştır. Giriş ve JWT üretiminin çalışması için en az 32 karakterlik bir `SecretKey` tanımlamanız gerekir.

Seçenek 1, User Secrets ile tanımlama (önerilen):

```powershell
cd SerieA.API
dotnet user-secrets set "Jwt:SecretKey" "SerieA_Super_Secret_Jwt_Key_2026_Minimum_32_Chars!"
cd ..
```

Seçenek 2, doğrudan `SerieA.API/appsettings.json` içine yazma:

```json
"Jwt": {
  "Issuer": "SerieA.API",
  "Audience": "SerieA.WebUI",
  "SecretKey": "SerieA_Super_Secret_Jwt_Key_2026_Minimum_32_Chars!",
  "ExpireMinutes": 240
}
```

#### API adresi

`SerieA.WebUI/appsettings.json` dosyasında API adresi varsayılan olarak `https://localhost:7283/api/` şeklindedir:

```json
"ApiSettings": {
  "BaseUrl": "https://localhost:7283/api/"
}
```

### 4. Projeyi çalıştırma

Uygulamanın tam çalışması için `SerieA.API` ve `SerieA.WebUI` projeleri aynı anda çalışmalıdır.

#### Seçenek A: Visual Studio 2022 (önerilen)

1. `SerieA.sln` dosyasını Visual Studio 2022 ile açın.
2. Solution Explorer'da `Solution 'SerieA'` üzerine sağ tıklayıp **Configure Startup Projects...** seçeneğini açın.
3. **Multiple startup projects** seçeneğini işaretleyin ve iki projeyi de `Start` olarak ayarlayın. İkisi için de profil `https` olmalıdır:
   - `SerieA.API`
   - `SerieA.WebUI`
4. **F5** veya **Start** ile iki projeyi birlikte başlatın.

#### Seçenek B: .NET CLI

İki ayrı terminal açın ve projeleri `https` profiliyle başlatın.

Terminal 1, backend API:

```powershell
dotnet run --project SerieA.API --launch-profile https
```

Terminal 2, frontend WebUI:

```powershell
dotnet run --project SerieA.WebUI --launch-profile https
```

| Uygulama | Adres |
| :--- | :--- |
| API | `https://localhost:7283` |
| Swagger UI | `https://localhost:7283/swagger` |
| Web arayüzü (public puan durumu) | `https://localhost:7015` |
| Yönetim paneli giriş ekranı | `https://localhost:7015/Account/Login` |

## Veritabanı içeriği ve giriş bilgileri

`SerieA.API/Data/seriea-data.sql` çalıştırıldığında veritabanına şu veriler yüklenir:

| Tablo | Kayıt sayısı | Açıklama |
| :--- | :---: | :--- |
| `Teams` | 20 | 2024/25 Serie A sezonundaki 20 takım (Inter, Milan, Juventus, Napoli, Roma, Lazio, Atalanta vb.), stadyum kapasiteleri, şehir/bölge, kuruluş yılı, renk kodları ve logolar |
| `Matches` | 40 | Oynanmış ve planlanmış örnek lig maçları, haftaları, skorları ve stadyum bilgileri |
| `MatchGoals` | 92 | Maçlara ait gol kayıtları ve dakika bilgileri |
| `MatchCards` | 129 | Maçlarda çıkan sarı ve kırmızı kart kayıtları |
| `Substitutions` | 247 | Maç içi giren/çıkan oyuncu değişikliği kayıtları |
| `Users` | 1 | Sistem yöneticisi (`admin@seriea.com`, rol: `Admin`) |

### Yönetim paneline giriş

Hazır SQL dosyasında `admin@seriea.com` e-posta adresli bir `Admin` kullanıcısı bulunur.

Yeni bir yönetici oluşturmak veya mevcut şifreyi sıfırlamak isterseniz, geliştirme ortamında `UsersController` üzerindeki `[Authorize(Roles = "Admin")]` özniteliğini geçici olarak `[AllowAnonymous]` yapın. Ardından Swagger (`https://localhost:7283/swagger`) üzerinden `POST /api/users` uç noktasını kullanarak kendi belirlediğiniz şifreyle (en az 8 karakter) yeni bir `Admin` kullanıcısı oluşturabilirsiniz.

## API uç noktaları

Yetki sütunundaki değerler:

| Değer | Anlamı |
| :--- | :--- |
| Herkese açık | Kimlik doğrulama gerekmez |
| `[Authorize]` | Giriş yapmış kullanıcı gerekir |
| `Admin` | Yalnızca `Admin` rolü |

### Kimlik doğrulama ve panel

| Metod | Uç nokta | Yetki | Açıklama |
| :---: | :--- | :---: | :--- |
| `POST` | `/api/auth/login` | Herkese açık | E-posta ve şifreyle giriş yapar, JWT Bearer token döner. |
| `GET` | `/api/dashboard` | `[Authorize]` | Yönetim paneli özet istatistiklerini, son ve gelecek maçları ile ilk 5 sırayı döner. |

### Puan durumu

| Metod | Uç nokta | Yetki | Açıklama |
| :---: | :--- | :---: | :--- |
| `GET` | `/api/standings?upToWeek={week}` | Herkese açık | Güncel veya belirtilen haftaya kadarki puan durumunu dinamik hesaplar. |

### Takımlar

| Metod | Uç nokta | Yetki | Açıklama |
| :---: | :--- | :---: | :--- |
| `GET` | `/api/teams` | Herkese açık | Takımları arama (`q`), bölge (`region`) ve durum (`status`) filtreleriyle listeler. |
| `GET` | `/api/teams/summary` | Herkese açık | Aktif takım sayısı, toplam stadyum kapasitesi ve en eski kulüp bilgisini döner. |
| `GET` | `/api/teams/{id}` | Herkese açık | ID'ye göre tek takımın detayını getirir. |
| `POST` | `/api/teams` | `[Authorize]` | Yeni takım ekler (en fazla 20 aktif takım kontrolüyle). |
| `PUT` | `/api/teams/{id}` | `[Authorize]` | Takım bilgilerini günceller. |
| `DELETE` | `/api/teams/{id}` | `[Authorize]` | Takımı pasife alır (soft delete, `Status = false`). |

### Maçlar

| Metod | Uç nokta | Yetki | Açıklama |
| :---: | :--- | :---: | :--- |
| `GET` | `/api/matches` | Herkese açık | Maçları hafta (`week`) ve takım (`team`) filtresiyle listeler. |
| `GET` | `/api/matches/week/{week}` | Herkese açık | Belirtilen haftanın maçlarını getirir. |
| `GET` | `/api/matches/weeks-summary` | Herkese açık | Tüm haftaların maç, gol ve durum özet istatistiklerini döner. |
| `GET` | `/api/matches/{id}` | Herkese açık | Maçın gol, kart ve oyuncu değişiklikleriyle birlikte tüm detayını getirir. |
| `POST` | `/api/matches` | `[Authorize]` | Yeni maç oluşturur. |
| `PUT` | `/api/matches/{id}` | `[Authorize]` | Maç bilgilerini, durumunu ve skorunu günceller. |
| `DELETE` | `/api/matches/{id}` | `[Authorize]` | Maçı ve ilişkili alt olaylarını siler. |

### Maç olayları

| Metod | Uç nokta | Yetki | Açıklama |
| :---: | :--- | :---: | :--- |
| `POST` | `/api/matches/{id}/goals` | `[Authorize]` | Maça gol olayı ekler. |
| `DELETE` | `/api/matches/goals/{goalId}` | `[Authorize]` | Gol kaydını siler. |
| `POST` | `/api/matches/{id}/cards` | `[Authorize]` | Maça sarı/kırmızı kart olayı ekler. |
| `DELETE` | `/api/matches/cards/{cardId}` | `[Authorize]` | Kart kaydını siler. |
| `POST` | `/api/matches/{id}/substitutions` | `[Authorize]` | Maça oyuncu değişikliği olayı ekler. |
| `DELETE` | `/api/matches/substitutions/{subId}` | `[Authorize]` | Oyuncu değişikliği kaydını siler. |

### Kullanıcılar

| Metod | Uç nokta | Yetki | Açıklama |
| :---: | :--- | :---: | :--- |
| `GET` | `/api/users` | `Admin` | Kullanıcıları rol, durum ve arama filtresiyle listeler. |
| `GET` | `/api/users/{id}` | `Admin` | Tek kullanıcının bilgisini getirir. |
| `POST` | `/api/users` | `Admin` | Yeni `Admin` veya `Editor` kullanıcısı oluşturur. |
| `PUT` | `/api/users/{id}` | `Admin` | Kullanıcı bilgilerini ve isteğe bağlı olarak şifresini günceller. |
| `DELETE` | `/api/users/{id}` | `Admin` | Kullanıcıyı sistemden siler. |