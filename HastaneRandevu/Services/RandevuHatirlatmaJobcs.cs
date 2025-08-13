using HastaneRandevu.Data;
using HastaneRandevu.Services; // IEmailService burada olmalı
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class RandevuHatirlatmaJob
{
    private readonly Context _db;
    private readonly IEmailService _email;

    public RandevuHatirlatmaJob(Context db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task GonderAsync()
    {
        var start = DateTime.Now.AddMinutes(0);
        var end = DateTime.Now.AddMinutes(2);

        var randevular = await _db.Randevular
            .Include(r => r.Hasta)
            .Include(r => r.Doktor)
            .Where(r => r.RandevuSaati >= start && r.RandevuSaati < end)
            //.Where(r => !r.Iptal) // varsa iptal kontrolü
            .Select(r => new
            {
                r.Id,
                r.RandevuSaati,
                HastaEmail = r.Hasta.Email,
                HastaAdSoyad = r.Hasta.AdSoyadi,
                DoktorAdSoyad = r.Doktor.Ad
            })
            .ToListAsync();

        foreach (var r in randevular)
        {
            if (string.IsNullOrWhiteSpace(r.HastaEmail)) continue;

            var subject = "Randevu Hatırlatma";
            var body =
$@"Merhaba {r.HastaAdSoyad},

{r.RandevuSaati:dd.MM.yyyy HH:mm} saatindeki randevunuz yaklaşıyor.
Lütfen zamanında hazır olun.

Bilgi amaçlı otomatik mesajdır.";

            await _email.SendEmailAsync(r.HastaEmail, subject, body);
        }
    }
}


