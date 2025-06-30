using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class TourPlannerContext(DbContextOptions<TourPlannerContext> options) : DbContext(options)
{
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourLog> TourLogs => Set<TourLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TourLog>()
            .HasOne(t => t.Tour)
            .WithMany(t => t.TourLogs)
            .HasForeignKey(t => t.TourId);
    }
}