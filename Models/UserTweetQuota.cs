namespace MiniTweeterBackend.Models
{
    public class UserTweetQuota
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int TweetsCount { get; set; }
        public bool HasUnlimited { get; set; }

        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
