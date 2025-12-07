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
            
        }
    }
}