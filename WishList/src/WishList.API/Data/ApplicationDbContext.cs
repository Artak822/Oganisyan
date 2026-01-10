using Microsoft.EntityFrameworkCore;
using WishList.API.Data.Models.Entities;

namespace WishList.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Wish> Wishes { get; set; }
    public DbSet<WishRecommendation> WishRecommendations { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure many-to-many relationship
        modelBuilder.Entity<WishRecommendation>()
            .HasOne(wr => wr.User)
            .WithMany(u => u.RecommendedWishes)
            .HasForeignKey(wr => wr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WishRecommendation>()
            .HasOne(wr => wr.Wish)
            .WithMany(w => w.RecommendedToUsers)
            .HasForeignKey(wr => wr.WishId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint for UserId + WishId combination
        modelBuilder.Entity<WishRecommendation>()
            .HasIndex(wr => new { wr.UserId, wr.WishId })
            .IsUnique();

        // Unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<ApiKey>()
            .HasIndex(ak => ak.Key)
            .IsUnique();
    }
}

