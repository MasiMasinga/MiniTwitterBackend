using System.ComponentModel.DataAnnotations;

namespace Comment.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TweetId { get; set; }
        
        [Required, MaxLength(280)]
        public string CommentMessage { get; set; }
        public int RetweetsCount { get; set; }
        public int LikesCount { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
