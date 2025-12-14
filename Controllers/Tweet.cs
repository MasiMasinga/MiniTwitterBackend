using Tweet.Dto;
using Tweet.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Tweet.Controller
{
    [ApiController]
    [Route("api/[controller]")]
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

            if (string.IsNullOrEmpty(tweet.PostMessage))
            {
                return BadRequest();
            }

            if (tweet.UserId <= 0)
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

    }
}