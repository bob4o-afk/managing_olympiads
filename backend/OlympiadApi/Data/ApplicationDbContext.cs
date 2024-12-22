using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OlympiadApi.Models;
using OlympiadApi.Helpers;


namespace OlympiadApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AcademicYear> AcademicYear { get; set; } = null!;
        public DbSet<Olympiad> Olympiads { get; set; } = null!;
        //public DbSet<Olympiad>? Olympiads { get; set; }
        public DbSet<Role> Roles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Olympiad>()
                .HasOne<AcademicYear>()
                .WithMany()
                .HasForeignKey(o => o.AcademicYearId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if AcademicYear is removed

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.RoleId);

                entity.Property(r => r.RoleName)
                    .HasColumnName("RoleName")
                    .IsRequired()
                    .HasMaxLength(255);

                // Apply value converter for Permissions
                var jsonConverter = new ValueConverter<Dictionary<string, object>?, string?>(
                    v => JsonSerializationHelper.SerializeToJson(v),
                    v => JsonSerializationHelper.DeserializeFromJson(v)
                );

                entity.Property(r => r.Permissions)
                    .HasColumnName("Permissions")
                    .HasColumnType("JSON")
                    .HasConversion(jsonConverter);
            });
        }
    }
}
