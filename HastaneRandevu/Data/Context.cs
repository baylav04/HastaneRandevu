using HastaneRandevu.Areas.Identity.Data;
using HastaneRandevu.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.Data
{
    public class Context : IdentityDbContext<HastaneRandevuUser>
    {
        internal object SeciliBrans;

        public Context(DbContextOptions<Context> options) : base(options)
        {
            
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=HastaneRandevuSistemi;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Randevu>()
                .HasOne(r => r.Poliklinik)
                .WithMany(p => p.Randevular)
                .HasForeignKey(r => r.PoliklinikId)
                .OnDelete(DeleteBehavior.Restrict);

            // 💡 Doktor + Tarih için UNIQUE INDEX
            modelBuilder.Entity<Randevu>()
                .HasIndex(r => new { r.DoktorId, r.RandevuSaati })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Poliklinik> Poliklinikler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<Doktor> Doktorlar { get; set; }
        public DbSet<Hasta> Hastalar { get; set; }
        public object Branslar { get; internal set; }
    }
}
