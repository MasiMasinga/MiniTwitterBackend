using CommentModel = Comment.Models.Comment;
using LikesModel = Likes.Models.Likes;
using PaymentModel = Payment.Models.Payments;
using RetweetModel = Retweet.Models.Retweet;
using TweetModel = Tweet.Models.Tweet;
using UserModel = User.Models.User;
using UserTweetQuotaModel = UserTweetQuota.Models.UserTweetQuota;
using Microsoft.EntityFrameworkCore;

namespace Tweet.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<LikesModel> Likes { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<RetweetModel> Retweets { get; set; }
        public DbSet<TweetModel> Tweets { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<UserTweetQuotaModel> UserTweetQuota { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<TweetModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.RetweetsCount).HasDefaultValue(0);
                entity.Property(e => e.LikesCount).HasDefaultValue(0);

                entity.HasOne<UserModel>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CommentModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.TweetId);
                entity.Property(e => e.RetweetsCount).HasDefaultValue(0);
                entity.Property(e => e.LikesCount).HasDefaultValue(0);

                entity.HasOne<UserModel>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<TweetModel>()
                    .WithMany()
                    .HasForeignKey(e => e.TweetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LikesModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.TargetType, e.TargetId });
                entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId }).IsUnique();

                entity.HasOne<UserModel>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RetweetModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.TargetType, e.TargetId });
                entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId }).IsUnique();

                entity.HasOne<UserModel>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaymentModel>(entity =>
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

                entity.HasOne<UserModel>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserTweetQuotaModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();
                entity.Property(e => e.TweetsCount).HasDefaultValue(0);
                entity.Property(e => e.HasUnlimited).HasDefaultValue(false);

                entity.HasOne<UserModel>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}