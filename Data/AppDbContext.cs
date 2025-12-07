using Microsoft.EntityFrameworkCore;
using MiniTweeterBackend.Models;

namespace MiniTweeterBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<Retweet> Retweets { get; set; }
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserTweetQuota> UserTweetQuota { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Tweet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.RetweetsCount).HasDefaultValue(0);
                entity.Property(e => e.LikesCount).HasDefaultValue(0);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.TweetId);
                entity.Property(e => e.RetweetsCount).HasDefaultValue(0);
                entity.Property(e => e.LikesCount).HasDefaultValue(0);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Tweet>()
                    .WithMany()
                    .HasForeignKey(e => e.TweetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Likes>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.TargetType, e.TargetId });
                entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId }).IsUnique();

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Retweet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.TargetType, e.TargetId });
                entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId }).IsUnique();

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Payments>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.TransactionId).IsUnique();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);
                
                entity.Property(e => e.ProviderResponse)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
                    .HasColumnType("jsonb");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserTweetQuota>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();
                entity.Property(e => e.TweetsCount).HasDefaultValue(0);
                entity.Property(e => e.HasUnlimited).HasDefaultValue(false);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}