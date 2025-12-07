using System.ComponentModel.DataAnnotations;

namespace MiniTweeterBackend.Models
{

    public class Retweet
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        public PostType TargetType { get; set; }

        [Required]
        public int TargetId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
