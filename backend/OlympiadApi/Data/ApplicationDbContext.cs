using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OlympiadApi.Models;
using OlympiadApi.Helpers;

namespace OlympiadApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<AcademicYear> AcademicYear { get; set; } = null!;
        public virtual DbSet<Olympiad> Olympiads { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRoleAssignment> UserRoleAssignments { get; set; } = null!;
        public virtual DbSet<StudentOlympiadEnrollment> StudentOlympiadEnrollment { get; set; } = null!;
        public virtual DbSet<UserToken> UserToken { get; set; }  = null!;

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
                    .HasConversion(
                        v => JsonSerializationHelper.SerializeToJson(v),
                        v => JsonSerializationHelper.DeserializeFromJson(v));
            });

            // Configure User entity with JSON fields and relationships
            var jsonConverter = new ValueConverter<Dictionary<string, object>?, string?>(
                v => JsonSerializationHelper.SerializeToJson(v),
                v => JsonSerializationHelper.DeserializeFromJson(v));

            modelBuilder.Entity<User>()
                .Property(u => u.PersonalSettings).HasColumnType("JSON").HasConversion(jsonConverter);
            modelBuilder.Entity<User>()
                .Property(u => u.Notifications).HasColumnType("JSON").HasConversion(jsonConverter);

            modelBuilder.Entity<User>()
                .HasOne(u => u.AcademicYear)
                .WithMany()
                .HasForeignKey(u => u.AcademicYearId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure UserRoleAssignment Relationships
            modelBuilder.Entity<UserRoleAssignment>()
                .HasOne(ura => ura.User)
                .WithMany()
                .HasForeignKey(ura => ura.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRoleAssignment>()
                .HasOne(ura => ura.Role)
                .WithMany()
                .HasForeignKey(ura => ura.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure StudentOlympiadEnrollment Relationships
            modelBuilder.Entity<StudentOlympiadEnrollment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentOlympiadEnrollment>()
                .HasOne(e => e.Olympiad)
                .WithMany()
                .HasForeignKey(e => e.OlympiadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentOlympiadEnrollment>()
                .HasOne(e => e.AcademicYear)
                .WithMany()
                .HasForeignKey(e => e.AcademicYearId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure JSONB for StatusHistory in StudentOlympiadEnrollment
            modelBuilder.Entity<StudentOlympiadEnrollment>()
                .Property(e => e.StatusHistory)
                .HasColumnType("JSONB")
                .HasConversion(
                    v => JsonSerializationHelper.SerializeToJson(v),
                    v => JsonSerializationHelper.DeserializeFromJson(v));

            modelBuilder.Entity<UserToken>()
                .HasOne(ut => ut.User)
                .WithMany()
                .HasForeignKey(ut => ut.UserId);

        }
    }
}
