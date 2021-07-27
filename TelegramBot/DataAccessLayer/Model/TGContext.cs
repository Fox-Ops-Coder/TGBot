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
            });

            modelBuilder.Entity<Vacancy>(entity =>
            {
                entity.Property(vacancy => vacancy.VacancyId)
                .ValueGeneratedOnAdd();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(pgConnection);
        }
    }
}