namespace Tweet.Dto
{
    public class CreateTweetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PostMessage { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}