# Serie A Futbol Ligi Yönetim ve İstatistik Platformu

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC%20%2B%20Web%20API-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-EF%20Core%208-CC2927?logo=microsoftsqlserver&logoColor=white)
![Auth](https://img.shields.io/badge/Auth-JWT%20%2B%20Cookie-orange)
![License](https://img.shields.io/badge/License-MIT-green)

Serie A Platformu, İtalya Serie A Futbol Ligi'ndeki takımları, haftalık fikstürleri, canlı ve tamamlanmış maç sonuçlarını, maç içi olayları (goller, sarı/kırmızı kartlar, oyuncu değişiklikleri) ve dinamik puan durumunu yönetmek ve göstermek için **.NET 8.0** ile geliştirilmiş bir full-stack web uygulamasıdır.

Proje birbirinden ayrık iki katmandan oluşur: RESTful Web API (`SerieA.API`) ve bu API'yi tüketen ASP.NET Core MVC web arayüzü (`SerieA.WebUI`). Uygulamanın iki yüzü vardır: futbolseverlere açık bir public istatistik ve fikstür portalı ile yetkili kullanıcılar (`Admin` ve `Editor`) için bir yönetim paneli.

## İçindekiler

1. [Neler yapıldı? (Özet)](#neler-yapıldı-özet)
2. [Mimari özeti](#mimari-özeti)
3. [Roller ve yetkiler](#roller-ve-yetkiler)
4. [Hızlı başlangıç](#hızlı-başlangıç)
5. [Uygulama ekran görüntüleri](#uygulama-ekran-görüntüleri)
6. [Detaylı dokümantasyon](#detaylı-dokümantasyon)
7. [Teknolojiler ve paket sürümleri](#teknolojiler-ve-paket-sürümleri)
8. [Kurulum](#kurulum)
9. [Yönetim paneline giriş](#yönetim-paneline-giriş)
10. [Sorun giderme](#sorun-giderme)
11. [Yol haritası](#yol-haritası)

---

## Neler yapıldı? (Özet)

- **Ziyaretçi Portalı (Public):** Oturum açmadan erişilebilen dinamik puan durumu (`upToWeek` ile geçmiş haftalara göre filtreleme), 38 haftalık etkileşimli fikstür ve uzatma dakikası (`45+2` vb.) destekli kronolojik maç merkezi.
- **Yönetim Paneli (Admin ve Editor):** Lig istatistikleri genel bakışı (Dashboard), 20 aktif takım sınırı ve ortak stadyum tekilleştirmeli (San Siro / Giuseppe Meazza) takım yönetimi, AJAX tabanlı maç/fikstür yönetimi, gerçek takım kadroları (`team-rosters.json`) ile gol/kart/oyuncu değişikliği yönetimi ve rol bazlı kullanıcı yönetimi.
- **Güvenlik ve Mimari:** EF Core Code-First ilişkisel veritabanı, tek tip `ApiResponse<T>` yanıt yapısı, merkezi hata yakalama (`ExceptionMiddleware` & `ApiExceptionHandler`), JWT + `HttpOnly` Cookie köprüsü (`AuthHeaderHandler`) ve CSRF koruması.

## Mimari özeti

```mermaid
flowchart LR
    B[Tarayıcı] -->|HttpOnly Cookie| W[SerieA.WebUI<br/>ASP.NET Core MVC]
    W -->|Bearer JWT<br/>AuthHeaderHandler| A[SerieA.API<br/>REST Web API]
    A -->|EF Core| D[(SQL Server)]
```

WebUI, giriş sonrası API'den aldığı JWT'yi `HttpOnly` cookie içinde saklar ve her API isteğine `Authorization: Bearer` başlığı olarak ekler. Ayrıntılar için [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) dosyasına bakın.

## Roller ve yetkiler

| Özellik | Ziyaretçi | Editor | Admin |
| :--- | :---: | :---: | :---: |
| Puan durumu, fikstür, maç detayı (public portal) | ✅ | ✅ | ✅ |
| Dashboard, takım / maç / hafta / olay yönetimi | ❌ | ✅ | ✅ |
| Kullanıcı ve rol yönetimi | ❌ | ❌ | ✅ |

---

## Hızlı başlangıç

Ön gereksinimler: .NET 8 SDK ve SQL Server (Express / Developer / LocalDB).

```powershell
git clone https://github.com/fatihaltuntas0/MYAcedemy-SerieA-Project.git
cd MYAcedemy-SerieA-Project
dotnet restore

# 1) Boş veritabanını oluşturun, sonra SerieA.API/Data/seriea-data.sql dosyasını SSMS'te çalıştırın
# 2) JWT anahtarını tanımlayın (kendi rastgele değerinizle)
dotnet user-secrets set "Jwt:SecretKey" "<en-az-32-karakterlik-rastgele-deger>" --project SerieA.API

# 3) İki projeyi ayrı terminallerde başlatın
dotnet run --project SerieA.API --launch-profile https
dotnet run --project SerieA.WebUI --launch-profile https
```

Ardından `https://localhost:7015` (public portal) veya `https://localhost:7015/Account/Login` (yönetim paneli) adresini açın. Demo giriş bilgileri: [Yönetim paneline giriş](#yönetim-paneline-giriş). Adımların ayrıntısı [Kurulum](#kurulum) bölümündedir.

---

## Uygulama ekran görüntüleri

Tüm ekran görüntüleri [`docs/images/`](docs/images/) dizininde yer almaktadır.

### 1. Ziyaretçi Portalı (Public Arayüz)

<details>
<summary><b>📊 Puan Durumu ve İstatistikler (4 Görsel)</b></summary>
<br>

| Puan Durumu — Öne Çıkanlar ve Dağılım | Puan Durumu — Lig Tablosu |
| :---: | :---: |
| ![Puan Durumu Genel](docs/images/SerieA-PuanDurumu.png) | ![Puan Durumu Tablo](docs/images/SerieA-PuanDurumu1.png) |
| **Puan Durumu — Alt Sıralar ve Kurallar** | **Haftaya Göre Filtrelenmiş Görünüm (2. Hafta)** |
| ![Puan Durumu Alt](docs/images/SerieA-PuanDurumu2.png) | ![Puan Durumu 2. Hafta](docs/images/SerieA-PuanDurumuHafta2.png) |

</details>

<details>
<summary><b>📅 Haftalık Fikstür ve Maç Merkezi (4 Görsel)</b></summary>
<br>

| Haftalık Fikstür ve İlerleme Durumu | Günlük Maç Listesi ve Hafta Özeti |
| :---: | :---: |
| ![Fikstür Üst](docs/images/SerieA-Fikstur.png) | ![Fikstür Alt](docs/images/SerieA-Fikstur1.png) |
| **Maç Merkezi — Skor Tabelası ve Akış Şeridi** | **Maç Merkezi — Zaman Çizelgesi ve İstatistikler** |
| ![Maç Detayı Üst](docs/images/SerieA-MacDetayi.png) | ![Maç Detayı Alt](docs/images/SerieA-MacDetayi1.png) |

</details>

---

### 2. Yönetim Paneli (Admin ve Editor)

<details>
<summary><b>🔐 Giriş ve Genel Bakış / Dashboard (2 Görsel)</b></summary>
<br>

| Yönetici Giriş Ekranı | Genel Bakış (Dashboard) |
| :---: | :---: |
| ![Admin Giriş](docs/images/AdminPanel-Giris.png) | ![Admin Genel Bakış](docs/images/AdminPanel-GenelBakis.png) |

</details>

<details>
<summary><b>🛡️ Takım Yönetimi (3 Görsel)</b></summary>
<br>

| Takım Listesi ve Lig Özeti Şeridi | Bölge / Arama Filtreli Görünüm |
| :---: | :---: |
| ![Admin Takımlar](docs/images/AdminPanel-Takimlar.png) | ![Admin Takımlar Filtreli](docs/images/AdminPanel-TakimlarFiltreli.png) |

| Yeni Takım Ekleme / Düzenleme Modalı |
| :---: |
| ![Admin Takım Ekleme](docs/images/AdminPanel-TakimEkleme.png) |

</details>

<details>
<summary><b>⚽ Maç, Fikstür ve Hafta Yönetimi (6 Görsel)</b></summary>
<br>

| Maç Listesi | Filtrelenmiş Maç Listesi |
| :---: | :---: |
| ![Admin Maçlar](docs/images/AdminPanel-Maclar.png) | ![Admin Maçlar Filtreli](docs/images/AdminPanel-MaclarFiltreli.png) |
| **Yeni Maç Ekleme (Başlamadı)** | **Canlı / Tamamlanan Maç Skor Girişi** |
| ![Admin Maç Ekleme Oynanmadı](docs/images/AdminPanel-MacEklemeOynanmadi.png) | ![Admin Maç Ekleme Canlı](docs/images/AdminPanel-MacEklemeCanli.png) |
| **Haftalar Genel Görünümü** | **Seçili Hafta Maç Detayları** |
| ![Admin Haftalar](docs/images/AdminPaneli-Haftalar.png) | ![Admin Haftalar Detay](docs/images/AdminPaneli-HaftalarDetay.png) |

</details>

<details>
<summary><b>🏆 Puan Durumu ve Kullanıcı Yönetimi (3 Görsel)</b></summary>
<br>

| Yönetim Paneli Puan Durumu | Döneme Göre Filtrelenmiş Puan Durumu |
| :---: | :---: |
| ![Admin Puan Durumu](docs/images/AdminPanel-PuanDurumu.png) | ![Admin Puan Durumu Filtreli](docs/images/AdminPanel-PuanDurumuFiltreli.png) |

| Kullanıcı ve Rol Yönetimi (`Admin` Rolü) |
| :---: |
| ![Admin Kullanıcılar](docs/images/AdminPanel-Kullanicilar.png) |

</details>

---

## Detaylı dokümantasyon

Projenin mimari detayları, klasör yapısı ve API uç noktaları okunabilirliği artırmak adına ayrı dokümanlara bölünmüştür:

- 📐 **[Mimari, Proje Yapısı ve Özellik Detayları (`docs/ARCHITECTURE.md`)](docs/ARCHITECTURE.md)**
  Klasör ağacı, teknik mimari kararlar, ziyaretçi portalı ve yönetim paneli ekranlarının detaylı iş kuralları.
- 🌐 **[API Uç Noktaları ve Veritabanı İçeriği (`docs/API-ENDPOINTS.md`)](docs/API-ENDPOINTS.md)**
  Tüm REST API rotaları, HTTP metodları, yetki seviyeleri ve hazır SQL veritabanı tablosu içerikleri.

---

## Teknolojiler ve paket sürümleri

Aşağıdaki sürümler, projeyi derlemek veya paketleri sıfırdan kurmak için gereken tam sürümlerdir.

| Bileşen | Değer |
| :--- | :--- |
| Hedef framework | .NET 8.0 (`net8.0`) |
| Veritabanı | Microsoft SQL Server (Express, Developer veya LocalDB) |

<details>
<summary><b>📦 NuGet paket listesi</b></summary>
<br>

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

### SerieA.WebUI paketleri

| Paket | Sürüm | Kullanım amacı |
| :--- | :---: | :--- |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | `8.0.23` | MVC Controller ve View şablon üretimi (scaffolding) |

</details>

Paket referansları `.csproj` dosyalarında hazır tanımlıdır. `dotnet restore` çalıştırmanız, tüm paketleri doğru sürümleriyle indirmek için yeterlidir.

---

## Kurulum

### Ön gereksinimler

1. [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --version` ile kontrol edebilirsiniz)
2. Microsoft SQL Server (Express, Developer veya MSSQLSERVER) ve SQL Server Management Studio (SSMS)
3. Visual Studio 2022 (önerilir), VS Code veya JetBrains Rider

### 1. Repoyu klonlama ve paketleri yükleme

```powershell
git clone https://github.com/fatihaltuntas0/MYAcedemy-SerieA-Project.git
cd MYAcedemy-SerieA-Project
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

User Secrets ile tanımlayın. Değeri kendiniz üretin ve paylaşmayın:

```powershell
# Rastgele anahtar üretmek için (PowerShell):
[Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 }) -as [byte[]])

dotnet user-secrets set "Jwt:SecretKey" "<ürettiğiniz-anahtar>" --project SerieA.API
```

> [!WARNING]
> `SecretKey` değerini `appsettings.json` içine yazıp commit etmeyin. Bu dosya repoda herkese açıktır; gerçek ortamlarda ortam değişkeni (`Jwt__SecretKey`) veya bir secret manager kullanın.

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

---

## Yönetim paneline giriş

Hazır SQL dosyası bir `Admin` kullanıcısı içerir:

| Alan | Değer |
| :--- | :--- |
| E-posta | `admin@seriea.com` |
| Şifre | `Admin123` |

> [!NOTE]
> Bu kullanıcı ve şifre **yalnızca demo / dummy veridir**; geliştirme ve tanıtım amaçlıdır. Herkese açık veya gerçek bir ortama kurulum yapacaksanız ilk girişten sonra **Kullanıcılar** ekranından şifreyi mutlaka değiştirin veya bu kullanıcıyı silin.

Yeni kullanıcı oluşturmak için `Admin` olarak giriş yapıp **Kullanıcılar** ekranını kullanın (API tarafında `POST /api/users`, yalnızca `Admin` rolü).

---

## Sorun giderme

| Belirti | Olası neden / çözüm |
| :--- | :--- |
| Veritabanı bağlantı hatası | `ConnectionStrings:Default` içindeki sunucu adını kontrol edin (`.\SQLEXPRESS`, `.`, `(localdb)\MSSQLLocalDB`). SQL Server servisinin çalıştığından emin olun. |
| `Cannot open database "SerieAProject"` | SQL betiğinden önce `CREATE DATABASE [SerieAProject];` komutunu çalıştırmayı unutmuş olabilirsiniz. |
| Giriş yaparken 500 hatası | `Jwt:SecretKey` boştur veya 32 karakterden kısadır. User Secrets ile tanımlayın. |
| WebUI açılıyor ama veri gelmiyor | API çalışmıyor olabilir. API'nin `https://localhost:7283` adresinde açık olduğunu ve `ApiSettings:BaseUrl` değerini kontrol edin. |
| HTTPS sertifika uyarısı | `dotnet dev-certs https --trust` komutunu çalıştırın. |

---

## Yol haritası

- [ ] Otomatik testler (birim ve entegrasyon testleri) eklemek
- [ ] İlk Admin kullanıcısı için geliştirme ortamı seed mekanizması
- [ ] Docker Compose ile tek komutla kurulum

> `SerieA.WebUI/seriea-template/` klasörü, arayüzün tasarım prototipi olan statik HTML şablonlarını içerir; uygulama çalışırken kullanılmaz, referans amaçlı tutulmaktadır.
