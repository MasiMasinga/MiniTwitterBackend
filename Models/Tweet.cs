using System.ComponentModel.DataAnnotations;

namespace MiniTweeterBackend.Models
{
    public class Tweet
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required, MaxLength(280)]
        public string PostMessage { get; set; }

        public int RetweetsCount { get; set; }
        public int LikesCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
