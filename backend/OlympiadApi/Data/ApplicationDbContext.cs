using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OlympiadApi.Models;
using OlympiadApi.Helpers;

namespace OlympiadApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AcademicYear> AcademicYear { get; set; } = null!;
        public DbSet<Olympiad> Olympiads { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Olympiad <-> AcademicYear Relationship
            modelBuilder.Entity<Olympiad>()
                .HasOne(o => o.AcademicYear)
                .WithMany()
                .HasForeignKey(o => o.AcademicYearId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Role entity with Permissions as JSON
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(r => r.RoleName).IsRequired().HasMaxLength(255);
                entity.Property(r => r.Permissions)
                    .HasColumnType("JSON")
                    .HasConversion(new ValueConverter<Dictionary<string, object>?, string?>(
                        v => JsonSerializationHelper.SerializeToJson(v),
                        v => JsonSerializationHelper.DeserializeFromJson(v)));
            });

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasOne(u => u.AcademicYear)
                .WithMany()
                .HasForeignKey(u => u.AcademicYearId)
                .OnDelete(DeleteBehavior.SetNull);

            // PersonalSettings and Notifications as JSON
            var jsonConverter = new ValueConverter<Dictionary<string, object>?, string?>(
                v => JsonSerializationHelper.SerializeToJson(v),
                v => JsonSerializationHelper.DeserializeFromJson(v));

            modelBuilder.Entity<User>()
                .Property(u => u.PersonalSettings).HasColumnType("JSON").HasConversion(jsonConverter);
            modelBuilder.Entity<User>()
                .Property(u => u.Notifications).HasColumnType("JSON").HasConversion(jsonConverter);
        }
    }
}
