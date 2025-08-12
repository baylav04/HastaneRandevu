using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using HastaneRandevu.Data;
using HastaneRandevu.Models;
using HastaneRandevu.Services;

namespace HastaneRandevu.Test
{
    public class RandevuTestleri
    {
        private Context CreateContext()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new Context(options);
        }

        [Fact]
        public void TestRandevuTarihiGecerliMi()
        {
            var servis = new RandevuServisi(null);
            var futureDate = DateTime.Now.AddDays(1);
            var pastDate = DateTime.Now.AddDays(-1);

            Assert.True(servis.RandevuTarihiGecerliMi(futureDate));
            Assert.False(servis.RandevuTarihiGecerliMi(pastDate));
        }

        [Fact]
        public async Task Test1AyniDoktorBirebirAyniSaatHata()
        {
            using var ctx = CreateContext();
            ctx.Randevular.Add(new Randevu { DoktorId = 1, RandevuSaati = new DateTime(2025, 8, 13, 10, 0, 0) });
            await ctx.SaveChangesAsync();

            var servis = new RandevuServisi(ctx);
            var cakisiyorMu = await servis.DoktorRandevusuCakisiyorMu(1, new DateTime(2025, 8, 13, 10, 0, 0));

            Assert.True(cakisiyorMu);
        }

        [Fact]
        public async Task Test2AyniDoktorKismiCakisiyorHata()
        {
            using var ctx = CreateContext();
            ctx.Randevular.Add(new Randevu { DoktorId = 1, RandevuSaati = new DateTime(2025, 8, 13, 10, 0, 0) });
            await ctx.SaveChangesAsync();

            var servis = new RandevuServisi(ctx);
            var cakisiyorMu = await servis.DoktorRandevusuCakisiyorMu(1, new DateTime(2025, 8, 13, 10, 15, 0));

            Assert.True(cakisiyorMu);
        }

        [Fact]
        public async Task Test3AyniDoktorArdisikBasarili()
        {
            using var ctx = CreateContext();
            ctx.Randevular.Add(new Randevu { DoktorId = 1, RandevuSaati = new DateTime(2025, 8, 13, 10, 0, 0) });
            await ctx.SaveChangesAsync();

            var servis = new RandevuServisi(ctx);
            var cakisiyorMu = await servis.DoktorRandevusuCakisiyorMu(1, new DateTime(2025, 8, 13, 10, 30, 0));

            Assert.False(cakisiyorMu);
        }

        [Fact]
        public async Task Test4FarkliDoktorAyniSaatBasarili()
        {
            using var ctx = CreateContext();
            ctx.Randevular.Add(new Randevu { DoktorId = 1, RandevuSaati = new DateTime(2025, 8, 13, 10, 0, 0) });
            await ctx.SaveChangesAsync();

            var servis = new RandevuServisi(ctx);
            var cakisiyorMu = await servis.DoktorRandevusuCakisiyorMu(2, new DateTime(2025, 8, 13, 10, 0, 0));

            Assert.False(cakisiyorMu);
        }
    }
}
