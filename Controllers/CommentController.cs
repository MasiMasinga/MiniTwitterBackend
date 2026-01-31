using Comment.Dto;
using Comment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Comment.Controller
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]

  public class CommentController : ControllerBase
  {
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
      _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto commentDto)
    {
      try
      {
        if (commentDto == null)
        {
          return BadRequest("Comment data is null.");
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
          return Unauthorized();
        }
        commentDto.UserId = userId;

        if (string.IsNullOrWhiteSpace(commentDto.CommentMessage))
        {
          return BadRequest("Comment content cannot be empty.");
        }

        if (commentDto.CommentMessage.Length > 280)
        {
          return BadRequest("Comment message exceeds the maximum length of 280 characters.");
        }

        var createdComment = await _commentService.CreateComment(commentDto);
        return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
      try
      {
        var comment = await _commentService.GetCommentById(id);
        return Ok(comment);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllComments()
    {
      try
      {
        var comments = await _commentService.GetAllComments();
        return Ok(comments);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentDto commentDto)
    {
      try
      {
        var updatedComment = await _commentService.UpdateComment(id, commentDto);
        return Ok(updatedComment);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
      try
      {
        var deletedComment = await _commentService.DeleteComment(id);
        return Ok(deletedComment);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    [HttpPut("{id}/like")]
    public async Task<IActionResult> LikeComment(int id)
    {
      try
      {
        var likedComment = await _commentService.LikeComment(id);
        return Ok(likedComment);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    [HttpPut("{id}/unlike")]
    public async Task<IActionResult> UnlikeComment(int id)
    {
      try
      {
        var unlikedComment = await _commentService.UnlikeComment(id);
        return Ok(unlikedComment);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    [HttpPut("{id}/retweet")]
    public async Task<IActionResult> RetweetComment(int id)
    {
      try
      {
        var retweetedComment = await _commentService.RetweetComment(id);
        return Ok(retweetedComment);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }
  }
}

