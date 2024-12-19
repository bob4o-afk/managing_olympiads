using Microsoft.EntityFrameworkCore;
using OlympiadApi.Models;

namespace OlympiadApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AcademicYear>? AcademicYear { get; set; }
        public DbSet<Olympiad>? Olympiads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the foreign key for Olympiad to AcademicYear
            modelBuilder.Entity<Olympiad>()
                .HasOne<AcademicYear>()
                .WithMany() // No navigation property
                .HasForeignKey(o => o.AcademicYearId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
