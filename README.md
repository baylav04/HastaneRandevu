# 🏥 Hastane Randevu Yönetim Sistemi
Bu proje, **ASP.NET Core MVC** teknolojisi kullanılarak geliştirilmiş modern bir web tabanlı hastane randevu sistemidir. Sistem, hastaların çevrimiçi olarak doktor ve poliklinik seçip randevu alabilmesini sağlarken, doktor ve yönetici rolleri için özel işlevler ve güvenlik mekanizmaları sunmaktadır. Amaç, sağlık hizmetlerinin dijitalleşmesini desteklemek ve kullanıcı deneyimini optimize etmektir. 
- <img width="1898" height="916" alt="anamenü" src="https://github.com/user-attachments/assets/7ab191a0-65b0-4d62-99d9-10b9575227a6" />
---
 ## 📚 Kullanılan Kütüphaneler ve Sürümleri
 | Kütüphane / Framework | Sürüm | Açıklama | 
 |---------------------------------------|---------|----------| 
 | Entity Framework Core | 9.0.7 | ORM ve veri tabanı işlemleri |
 | Entity Framework Core InMemory | 9.0.7 | Test ortamında bellek içi veri tabanı |
 | Hangfire | 1.8.21 | Zamanlanmış görevler (randevu hatırlatma) |
 | Microsoft Identity | 8.0.11 | Kimlik doğrulama ve yetkilendirme | 
 | AutoMapper | 12.0.1 | Nesneler arası veri eşleme | 
 | Bootstrap | | UI tasarımı |
 | jQuery | | Frontend işlemleri |
 | AdminLTE | | Yönetim paneli arayüzü | 
 
 ## 📌 Temel Özellikler ve Fonksiyonel Kapasite
 
### 👤 **Rol Bazlı Kimlik Doğrulama ve Yetkilendirme**
 - Kullanıcılar Hasta, Doktor veya Yönetici (Admin) rollerine göre farklı erişim yetkilerine sahiptir.
   
### 📅 **Randevu Yönetimi**
 - Randevu oluşturma, listeleme, güncelleme ve iptal işlemleri
 - Aktif ve geçmiş randevuların takip edilebilmesi
 - <img width="1906" height="906" alt="aktifrandevular" src="https://github.com/user-attachments/assets/82adecce-ba62-4ca8-bb11-042919d95a19" />
 
### ⌛ **Zamanlanmış Hatırlatma Servisi**
 - Arka planda çalışan servis aracılığıyla, kullanıcıya e-posta ile randevu hatırlatmaları gönderilir
   
### 🔐 **Gelişmiş Güvenlik Mekanizmaları**
 - Şifre yenileme ve güçlü doğrulama
 - Google reCAPTCHA v2 ile bot saldırılarına karşı koruma
   
### 🎯 **Yazılım Mimarisi ve Kod Kalitesi**
 - DTO (Data Transfer Object) kullanımıyla veri taşımada güvenli ve temiz kod mimarisi
 - Middleware ile global hata yönetimi ve loglama
   
   ---
  ## 🛠️ Kullanılan Teknolojiler 
  
 ### 🔧 Backend - **ASP.NET Core MVC (.NET 7+)** – Web uygulama çatısı
   - **Entity Framework Core (Code First)** – Veritabanı yönetimi ve ORM 
   - **SQL Server** – Veritabanı sunucusu
   - **AutoMapper** – Model ve DTO dönüşümleri
   - **FluentValidation** – Model doğrulama ve iş kuralları
   - **Middleware** – Global hata yönetimi ve loglama 
   - **Background Service** – Zamanlanmış randevu hatırlatma görevleri
## ✉️ Güvenlik ve Erişim
-  **SMTP E-Posta Servisi:** Şifre sıfırlama ve bildirim e-postalarının güvenli şekilde gönderilmesini sağlar.

-  **Google reCAPTCHA v2:** Formları ve giriş ekranlarını botlara karşı korur, spam ve otomatik saldırıları engeller.
     
 ### 🎨 Frontend
 - **Razor View Engine (CSHTML)** – Sunucu taraflı dinamik içerik
- **Bootstrap 5** – Responsive tasarım ve grid sistemi
- **jQuery** – Dinamik etkileşimler ve doğrulamalar
     
 ### 📦 Yardımcı Araçlar
 -  **LibMan** – Frontend kütüphanelerinin yönetimi
 -  **appsettings.json** – Uygulama yapılandırmalarının merkezi yönetimi
---
     
 ## 🧭 Sistem Mimarisi ve Kullanıcı Rolleri
 ### 🔹 Hasta
  - Kayıt ve giriş işlemleri
  - Doktor ve poliklinik seçerek randevu oluşturma
  - Geçmiş ve aktif randevuların detaylı görüntülenmesi
  - <img width="1905" height="970" alt="hastagiris" src="https://github.com/user-attachments/assets/4fe19658-96b6-4528-874a-76921623c93f" />
  - <img width="1902" height="902" alt="Doktorlar" src="https://github.com/user-attachments/assets/8f534804-2e51-4a5d-93bf-08cf841f2f2d"/>

 ### 🔹 Yönetici (Admin)
  - Hasta ve doktor yönetimi
  - Yeni poliklinik ve doktor ekleme, güncelleme ve silme işlemleri
  - Sistem genelinde istatistik ve raporlama
  - <img width="1902" height="910" alt="admin" src="https://github.com/user-attachments/assets/ed66fd84-dc28-4a35-a1f0-d9af433b5b8b"/>

 ## 📁 Proje Dosya Yapısı (Özet) 
 - Controllers/ → Rol bazlı iş mantığı ve endpointler
 -  Models/ → Veritabanı tabloları ve ViewModel yapıları
 -  Views/ → Razor sayfaları (Admin, Doktor, Hasta, Randevu)
 -  Middlewares/ → Global hata yakalama ve loglama
 -  DTOs/ → Veri transfer nesneleri - Data/ → DbContext ve seed verileri
 -  Mapping/ → AutoMapper profilleri - Migrations/ → Veritabanı migration dosyaları
 -  Validators/ → FluentValidation kuralları

 ## 3️⃣ Ayar Dosyası (appsettings.json)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=HastaneRandevuDb;Trusted_Connection=True"
},
"EmailSettings": {
  "SmtpServer": "smtp.example.com",
  "Port": 587,
  "Sender": "noreply@hastanerandevu.com",
  "Password": "email-password"
},
"GoogleReCaptcha": {
  "SiteKey": "your-site-key",
  "SecretKey": "your-secret-key"
}
```
### SMTP ve reCAPTCHA bilgilerinizi güncel ve geçerli değerlerle doldurunuz.
## 📸 Uygulama Sayfa Yapısı 
- Admin/ → Hasta ve doktor listesi, kullanıcı yönetimi 
- Hastas/ → Randevu oluşturma, listeleme, profil güncelleme 
- Randevus/ → Randevu detayları, güncelleme ve silme 
- Doktors/ → Doktor paneli ve sınırlı erişim 
- Shared/ → Layout ve ortak bileşenler 
--- 
## 🧪 Geliştirici Notları 
- Middleware ile global hata yönetimi ve loglama sağlanmıştır 
- DTO ve ViewModel ayrımıyla veri aktarımı güvenli ve temiz bir şekilde yapılmaktadır 
- FluentValidation ile sunucu taraflı model doğrulama sağlanır 
- Background Service aracılığıyla randevu günü e-posta hatırlatmaları gönderilir.
--- 
## 📄 Lisans 
Bu proje açık kaynak kodludur ve eğitim / geliştirme amaçlı kullanım için uygundur. Ticari kullanım için ilgili izinlerin alınması önerilir.

--- 
## 📬 İletişim 
- Proje ile ilgili öneri, soru veya katkılar için:
### E-posta: baylavkadriye@gmail.com
