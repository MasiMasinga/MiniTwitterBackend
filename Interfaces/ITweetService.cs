using Tweet.Dto;

namespace Tweet.Interfaces
{
    public interface ITweetService
    {
        Task<CreateTweetDto> CreateTweet(CreateTweetDto tweet);
        Task<List<TweetDto>> GetAllTweets();
        Task<TweetDto> GetTweetById(int id);
        Task<TweetDto> UpdateTweet(int id, TweetDto tweet);
        Task<TweetDto> DeleteTweet(int id);
    }
}