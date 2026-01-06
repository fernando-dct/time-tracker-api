using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Core.Entities;

namespace TimeTracker.Api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<AdjustmentRequest> AdjustmentRequests => Set<AdjustmentRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).IsRequired();
            entity.Property(x => x.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.Timestamp });
            entity.Property(x => x.Type).IsRequired();
        });

        modelBuilder.Entity<AdjustmentRequest>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Reason).IsRequired();
            entity.Property(x => x.Status).IsRequired();
        });
    }
}