using HastaneRandevu.Models;

namespace HastaneRandevu.Data
{
    public class InitializeDatabase
    {
        public static void Initialize(Context context)
        {
            if (context.Database.EnsureCreated())
            {
                // Veritabanı oluşturulduğunda yapılacak işlemler
                Console.WriteLine("Veritabanı oluşturuldu.");
            }
            else
            {
                // Veritabanı zaten varsa yapılacak işlemler
                Console.WriteLine("Veritabanı zaten mevcut.");
            }

            // Veritabanı başlangıç verilerini ekleme
            if (!context.Poliklinikler.Any())
            {
                context.Poliklinikler.AddRange(
                    new Poliklinik { PoliklinikAdi = "Dahiliye" },
                    new Poliklinik { PoliklinikAdi = "Cerrahi" },
                    new Poliklinik { PoliklinikAdi = "Çocuk Sağlığı" }
                );
                context.SaveChanges();
            }

            // Doktorlar ekleme
            if (!context.Doktorlar.Any())
            {
                context.Doktorlar.AddRange(
                    new Doktor { Ad = "Dr. Ahmet Yılmaz", PoliklinikId = 1 },
                    new Doktor { Ad = "Dr. Ayşe Demir", PoliklinikId = 2 },
                    new Doktor { Ad = "Dr. Mehmet Can", PoliklinikId = 3 }
                );
                context.SaveChanges();
            }

            // Hastalar ekleme
            if (!context.Hastalar.Any())
            {
                context.Hastalar.AddRange(
                    new Hasta { AdSoyadi = "Ali Veli", Parola = "aliveli", TCKimlikNo = "12345678901"},
                    new Hasta { AdSoyadi = "Ayşe Fatma", Parola = "aysefatma", TCKimlikNo = "10987654321"}
                );
                context.SaveChanges();
            }

            // Randevular ekleme
            if (!context.Randevular.Any())
            {
                context.Randevular.AddRange(
                    new Randevu { HastaId = 1, DoktorId = 1, PoliklinikId = 1, RandevuSaati = DateTime.Now.AddDays(1) },
                    new Randevu { HastaId = 2, DoktorId = 2, PoliklinikId = 2, RandevuSaati = DateTime.Now.AddDays(2) },
                    new Randevu { HastaId = 1, DoktorId = 3, PoliklinikId = 3, RandevuSaati = DateTime.Now.AddDays(3) }
                );
                context.SaveChanges();
            }

        }
    }
}
