using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Model
{
    public sealed class TGContext : DbContext
    {
        private readonly string pgConnection;

        public DbSet<Profession> Professions { get; set; }
        public DbSet<Cource> Cources { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<VacancyTagRecord> VacancyTagRecords { get; set; }
        public DbSet<CourceTagRecord> CourceTagRecords { get; set; }

        public TGContext(string pgConnection) => this.pgConnection = pgConnection;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profession>(entity =>
            {
                entity.Property(profession => profession.ProfessionId)
                .ValueGeneratedOnAdd();

                entity.HasMany(profession => profession.Cources)
                .WithOne(cource => cource.Profession)
                .HasForeignKey(cource => cource.ProfessionId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(profession => profession.Vacancies)
                .WithOne(vacancy => vacancy.Profession)
                .HasForeignKey(vacancy => vacancy.ProfessionId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cource>(entity =>
            {
                entity.Property(cource => cource.CourceId)
                .ValueGeneratedOnAdd();

                entity.HasMany(cource => cource.CourceTagRecords)
                .WithOne(record => record.Cource)
                .HasForeignKey(record => record.CourceId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Vacancy>(entity =>
            {
                entity.Property(vacancy => vacancy.VacancyId)
                .ValueGeneratedOnAdd();

                entity.HasMany(vacancy => vacancy.VacancyTagRecords)
                .WithOne(record => record.Vacancy)
                .HasForeignKey(record => record.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VacancyTagRecord>(entity =>
            {
                entity.Property(record => record.RecordId)
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<CourceTagRecord>(entity =>
            {
                entity.Property(record => record.RecordId)
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(tag => tag.TagId)
                .ValueGeneratedOnAdd();

                entity.HasMany(tag => tag.CourceTagRecords)
                .WithOne(record => record.Tag)
                .HasForeignKey(record => record.TagId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(tag => tag.VacancyTagRecords)
                .WithOne(record => record.Tag)
                .HasForeignKey(record => record.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(pgConnection);
        }
    }
}