using HastaneRandevu.Areas.Identity.Data;
using HastaneRandevu.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.Data
{
    public class Context : IdentityDbContext<HastaneRandevuUser>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Poliklinik - Randevu ilişkisi
            modelBuilder.Entity<Randevu>()
                .HasOne(r => r.Poliklinik)
                .WithMany(p => p.Randevular)
                .HasForeignKey(r => r.PoliklinikId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doktor + RandevuSaati UNIQUE INDEX
            modelBuilder.Entity<Randevu>()
                .HasIndex(r => new { r.DoktorId, r.RandevuSaati })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Poliklinik> Poliklinikler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<Doktor> Doktorlar { get; set; }
        public DbSet<Hasta> Hastalar { get; set; }

       
    }
}
