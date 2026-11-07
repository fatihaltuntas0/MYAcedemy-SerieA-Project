# API Uç Noktaları ve Veritabanı İçeriği

[← Ana Sayfaya Dön (README.md)](../README.md)

Bu doküman, `SerieA.API` projesinde yer alan tüm RESTful uç noktalarını, yetkilendirme seviyelerini ve hazır veritabanı (`seriea-data.sql`) tablo içeriklerini listeler.

## İçindekiler

1. [API uç noktaları](#api-uç-noktaları)
   - [Kimlik doğrulama ve panel](#kimlik-doğrulama-ve-panel)
   - [Puan durumu](#puan-durumu)
   - [Takımlar](#takımlar)
   - [Maçlar](#maçlar)
   - [Maç olayları](#maç-olayları)
   - [Kullanıcılar](#kullanıcılar)
2. [Veritabanı içeriği (`seriea-data.sql`)](#veritabanı-içeriği-seriea-datasql)

---

## API uç noktaları

Yetki sütunundaki değerler:

| Değer | Anlamı |
| :--- | :--- |
| Herkese açık | Kimlik doğrulama gerekmez |
| `[Authorize]` | Giriş yapmış kullanıcı gerekir (`Admin` veya `Editor`) |
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

---

## Veritabanı içeriği (`seriea-data.sql`)

`SerieA.API/Data/seriea-data.sql` çalıştırıldığında veritabanına şu veriler yüklenir:

| Tablo | Kayıt sayısı | Açıklama |
| :--- | :---: | :--- |
| `Teams` | 20 | 2024/25 Serie A sezonundaki 20 takım (Inter, Milan, Juventus, Napoli, Roma, Lazio, Atalanta vb.), stadyum kapasiteleri, şehir/bölge, kuruluş yılı, renk kodları ve logolar |
| `Matches` | 40 | Oynanmış ve planlanmış örnek lig maçları, haftaları, skorları ve stadyum bilgileri |
| `MatchGoals` | 92 | Maçlara ait gol kayıtları ve dakika bilgileri |
| `MatchCards` | 129 | Maçlarda çıkan sarı ve kırmızı kart kayıtları |
| `Substitutions` | 247 | Maç içi giren/çıkan oyuncu değişikliği kayıtları |
| `Users` | 1 | Sistem yöneticisi (`admin@seriea.com`, rol: `Admin`) |

