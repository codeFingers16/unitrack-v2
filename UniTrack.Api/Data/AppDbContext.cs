using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniTrack.Api.Models;

namespace UniTrack.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser>(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Assignment> Assignments => Set<Assignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);   // required for Identity tables

        modelBuilder.Entity<Course>(e =>
        {
            e.Property(c => c.Code).IsRequired().HasMaxLength(20);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(c => new { c.UserId, c.Code }).IsUnique();   // unique per user
            e.HasOne(c => c.User)
             .WithMany()
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Assignment>(e =>
        {
            e.Property(a => a.Title).IsRequired().HasMaxLength(200);
            e.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(a => a.DueDate);
            e.HasOne(a => a.Course)
             .WithMany(c => c.Assignments)
             .HasForeignKey(a => a.CourseId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}