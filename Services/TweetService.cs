using Tweet.Dto;
using Tweet.Data;
using Microsoft.EntityFrameworkCore;
using Tweet.Interfaces;
using TweetModel = Tweet.Models.Tweet;
using UserModel = User.Models.User;

namespace Tweet.Services
{
    public class TweetService : ITweetService
    {
        private readonly AppDbContext _context;

        public TweetService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateTweetDto> CreateTweet(CreateTweetDto tweet)
        {

            var testUser = new UserModel
            {
                Username = "testuser1",
                FirstName = "Test",
                LastName = "User",
                Email = "test1@example.com",
                Password = "test123",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.User.Add(testUser);
            await _context.SaveChangesAsync();
            var userId = testUser.Id;

            try
            {
                var newTweet = new TweetModel
                {
                    PostMessage = tweet.PostMessage,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Tweets.Add(newTweet);
                await _context.SaveChangesAsync();

                return new CreateTweetDto
                {
                    Id = newTweet.Id,
                    UserId = newTweet.UserId,
                    PostMessage = tweet.PostMessage,
                    CreatedAt = newTweet.CreatedAt,
                    UpdatedAt = newTweet.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TweetDto>> GetAllTweets()
        {
            return await _context.Tweets
                .Select(t => new TweetDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    PostMessage = t.PostMessage,
                    RetweetsCount = t.RetweetsCount,
                    LikesCount = t.LikesCount,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToListAsync();
        }

        public async Task<TweetDto?> GetTweetById(int id)
        {
            return await _context.Tweets
                .Where(t => t.Id == id)
                .Select(t => new TweetDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    PostMessage = t.PostMessage,
                    RetweetsCount = t.RetweetsCount,
                    LikesCount = t.LikesCount,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).FirstOrDefaultAsync();
        }

        public async Task<TweetDto?> UpdateTweet(int id, TweetDto tweet)
        {
            try
            {
                var updatedTweet = await _context.Tweets.FindAsync(id);

                if (updatedTweet == null)
                {
                    return null!;
                }

                updatedTweet.PostMessage = tweet.PostMessage;
                updatedTweet.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                tweet.Id = updatedTweet.Id;
                tweet.CreatedAt = updatedTweet.CreatedAt;
                tweet.UpdatedAt = updatedTweet.UpdatedAt;
                return tweet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TweetDto?> DeleteTweet(int id)
        {
            try
            {
                var deletedTweet = await _context.Tweets.FindAsync(id);

                if (deletedTweet == null)
                {
                    return null!;
                }

                var tweet = new TweetDto
                {
                    Id = deletedTweet.Id,
                    PostMessage = deletedTweet.PostMessage,
                    CreatedAt = deletedTweet.CreatedAt,
                    UpdatedAt = deletedTweet.UpdatedAt
                };

                _context.Tweets.Remove(deletedTweet);
                await _context.SaveChangesAsync();

                return tweet;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}