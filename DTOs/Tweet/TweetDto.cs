namespace Tweet.Dto
{
    public class TweetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PostMessage { get; set; } = string.Empty;
        public int RetweetsCount { get; set; }
        public int LikesCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}