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
        Task<TweetDto> LikeTweet(int id);
        Task<TweetDto> UnlikeTweet(int id);
        Task<TweetDto> RetweetTweet(int id);
    }
}