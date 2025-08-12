using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HastaneRandevu.Data;

namespace HastaneRandevu.Services
{
    public class RandevuServisi
    {
        private readonly Context _context;

        public RandevuServisi(Context context)
        {
            _context = context;
        }

        // Geçmiş tarih kontrolü
        public bool RandevuTarihiGecerliMi(DateTime randevuSaati)
        {
            return randevuSaati > DateTime.Now;
        }

        // Çakışma kontrolü (30 dakika sabit süre)
        public async Task<bool> DoktorRandevusuCakisiyorMu(int doktorId, DateTime yeniBaslangic)
        {
            var yeniBitis = yeniBaslangic.AddMinutes(30);

            return await _context.Randevular.AnyAsync(r =>
                r.DoktorId == doktorId &&
                yeniBaslangic < r.RandevuSaati.AddMinutes(30) && 
                r.RandevuSaati < yeniBitis
            );
        }
    }
}

