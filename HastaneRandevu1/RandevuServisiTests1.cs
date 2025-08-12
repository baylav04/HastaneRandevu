using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using HastaneRandevu.Data;
using HastaneRandevu.Models;
using HastaneRandevu.Services;

namespace HastaneRandevu.Tests
{
    public class RandevuServisiTests
    {
        private async Task<Context> GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new Context(options);

            // Örnek doktor ve randevu ekle
            context.Doktorlar.Add(new Doktor { Id = 1, Ad = "Dr. Ahmet" });
            context.Randevular.Add(new Randevu
            {
                Id = 1,
                DoktorId = 1,
                RandevuSaati = new DateTime(2025, 8, 11, 10, 0, 0)
            });

            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task AynıDoktorAyniSaatRandevuOlmamalı()
        {
            var context = await GetInMemoryContext();
            var servis = new RandevuServisi(context);

            bool sonuc = await servis.DoktorAyniSaatRandevusuVarMi(1, new DateTime(2025, 8, 11, 10, 0, 0));
            Assert.True(sonuc); // Aynı doktor ve aynı saatte randevu var, sonuç True olmalı
        }

        [Fact]
        public void RandevuGecmisTarihOlamaz()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new Context(options);
            var servis = new RandevuServisi(context);

            var gecmisTarih = DateTime.Now.AddDays(-1);
            var sonuc = servis.RandevuTarihiGecerliMi(gecmisTarih);

            Assert.False(sonuc); // Geçmiş tarih, hata olmalı
        }
    }
}
