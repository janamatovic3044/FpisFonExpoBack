using FONEXPO2024.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FONEXPO2024.DataAccess
{
    public class FonExpoDbContext : DbContext
    {
        public FonExpoDbContext(DbContextOptions<FonExpoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Manifestacija> Manifestacije { get; set; }
        public DbSet<ExpoDan> ExpoDani { get; set; }
        public DbSet<Izlozba> Izlozbe { get; set; }
        public DbSet<CenaDana> CeneDana { get; set; }
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Prijava> Prijave { get; set; }
        public DbSet<PrijavaDan> PrijaveDana { get; set; }
        public DbSet<PromoKod> PromoKodovi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for PrijavaDan
            modelBuilder.Entity<PrijavaDan>()
                .HasKey(pd => new { pd.PrijavaID, pd.ExpoDanID });

            // Prijava ↔ PrijavaDan relations
            modelBuilder.Entity<PrijavaDan>()
                .HasOne(pd => pd.Prijava)
                .WithMany(p => p.Dani)
                .HasForeignKey(pd => pd.PrijavaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PrijavaDan>()
                .HasOne(pd => pd.ExpoDan)
                .WithMany(ed => ed.Prijave)
                .HasForeignKey(pd => pd.ExpoDanID)
                .OnDelete(DeleteBehavior.Cascade);

            // Manifestacija ↔ ExpoDan
            modelBuilder.Entity<ExpoDan>()
                .HasOne(ed => ed.Manifestacija)
                .WithMany(m => m.ExpoDani)
                .HasForeignKey(ed => ed.ManifestacijaID)
                .OnDelete(DeleteBehavior.Cascade);

            // ExpoDan ↔ Izlozba
            modelBuilder.Entity<Izlozba>()
                .HasOne(i => i.ExpoDan)
                .WithMany(ed => ed.Izlozbe)
                .HasForeignKey(i => i.ExpoDanID)
                .OnDelete(DeleteBehavior.Cascade);

            // ExpoDan ↔ CenaDana
            modelBuilder.Entity<CenaDana>()
                .HasOne(c => c.ExpoDan)
                .WithMany(ed => ed.CeneDana)
                .HasForeignKey(c => c.ExpoDanID)
                .OnDelete(DeleteBehavior.Cascade);

            // Prijava ↔ Korisnik
            modelBuilder.Entity<Prijava>()
                .HasOne(p => p.Korisnik)
                .WithMany(k => k.Prijave)
                .HasForeignKey(p => p.KorisnikID)
                .OnDelete(DeleteBehavior.Cascade);

            // Prijava → PromoKod (No cascade to avoid multiple cascade paths)
            modelBuilder.Entity<Prijava>()
                .HasOne(p => p.PromoKod)
                .WithMany()
                .HasForeignKey(p => p.PromoKodID)
                .OnDelete(DeleteBehavior.Restrict);

            // PromoKod → GenerisanOdPrijava
            modelBuilder.Entity<PromoKod>()
                .HasOne(pk => pk.GenerisanOdPrijava)
                .WithMany()
                .HasForeignKey(pk => pk.GenerisanOdPrijavaID)
                .OnDelete(DeleteBehavior.Cascade);

            // PromoKod → IskoriscenOdPrijava
            modelBuilder.Entity<PromoKod>()
                .HasOne(pk => pk.IskoriscenOdPrijava)
                .WithMany()
                .HasForeignKey(pk => pk.IskoriscenOdPrijavaID)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique indexes
            modelBuilder.Entity<Prijava>()
                .HasIndex(p => p.Token)
                .IsUnique();

            modelBuilder.Entity<PromoKod>()
                .HasIndex(pk => pk.Kod)
                .IsUnique();

            modelBuilder.Entity<Korisnik>()
                .HasIndex(k => k.Email)
                .IsUnique();

            // Decimal precision
            modelBuilder.Entity<CenaDana>()
                .Property(c => c.Cena)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Prijava>()
                .Property(p => p.OriginalPrice)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Prijava>()
                .Property(p => p.FinalPrice)
                .HasColumnType("decimal(10,2)");

            // Default values
            modelBuilder.Entity<Prijava>()
                .Property(p => p.StatusPrijave)
                .HasDefaultValue("Aktivna");

            modelBuilder.Entity<Prijava>()
                .Property(p => p.DatumPrijave)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Korisnik>()
                .Property(k => k.EmailPotvrdjen)
                .HasDefaultValue(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
