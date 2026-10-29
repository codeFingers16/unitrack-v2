using Microsoft.EntityFrameworkCore;
using UniTrack.Api.Models;

namespace UniTrack.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Assignment> Assignments => Set<Assignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(e =>
        {
            e.Property(c => c.Code).IsRequired().HasMaxLength(20);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(c => c.Code).IsUnique();
        });

        modelBuilder.Entity<Assignment>(e =>
        {
            e.Property(a => a.Title).IsRequired().HasMaxLength(200);
            e.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(a => a.DueDate);   // for the deadline queries in milestone 2
            e.HasOne(a => a.Course)
             .WithMany(c => c.Assignments)
             .HasForeignKey(a => a.CourseId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}