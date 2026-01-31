using Tweet.Dto;
using Tweet.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Tweet.Controller
{
    [ApiController]
    [Route("api/tweet")]
    [Authorize]
    public class TweetController : ControllerBase
    {
        private readonly ITweetService _tweetService;

        public TweetController(ITweetService tweetService)
        {
            _tweetService = tweetService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateTweetDto>> CreateTweet([FromBody] CreateTweetDto tweet)
        {

            if (tweet == null!)
            {
                return BadRequest();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            tweet.UserId = userId;

            if (string.IsNullOrEmpty(tweet.PostMessage))
            {
                return BadRequest();
            }

            if (tweet.PostMessage.Length > 280)
            {
                return BadRequest();
            }

            var createdTweet = await _tweetService.CreateTweet(tweet);

            return CreatedAtAction(nameof(GetTweetById), new { id = createdTweet }, createdTweet);
        }

        [HttpGet]
        public async Task<ActionResult<List<TweetDto>>> GetAllTweets()
        {
            var tweets = await _tweetService.GetAllTweets();

            return Ok(tweets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TweetDto>> GetTweetById(int id)
        {
            var tweet = await _tweetService.GetTweetById(id);

            return Ok(tweet);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TweetDto>> UpdateTweet(int id, [FromBody] TweetDto todo)
        {
            var updatedTweet = await _tweetService.UpdateTweet(id, todo);

            return Ok(updatedTweet);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TweetDto>> DeleteTweet(int id)
        {
            var deleteTweet = await _tweetService.DeleteTweet(id);

            return Ok(deleteTweet);
        }

        [HttpPut("{id}/like")]
        public async Task<ActionResult<TweetDto>> LikeTweet(int id)
        {
            try
            {
                var likedTweet = await _tweetService.LikeTweet(id);
                return Ok(likedTweet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}/unlike")]
        public async Task<ActionResult<TweetDto>> UnlikeTweet(int id)
        {
            try
            {
                var unlikedTweet = await _tweetService.UnlikeTweet(id);
                return Ok(unlikedTweet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}/retweet")]
        public async Task<ActionResult<TweetDto>> RetweetTweet(int id)
        {
            try
            {
                var retweetedTweet = await _tweetService.RetweetTweet(id);
                return Ok(retweetedTweet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}