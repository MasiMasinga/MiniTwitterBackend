using Comment.Dto;
using Tweet.Data;
using Comment.Interfaces;
using Microsoft.EntityFrameworkCore;
using CommentModel = Comment.Models.Comment;
using UserModel = User.Models.User;
using TweetModel = Tweet.Models.Tweet;

namespace Comment.Services
{
  public class CommentService : ICommentService
  {
    private readonly AppDbContext _context;

    public CommentService(AppDbContext context)
    {
      _context = context;
    }

    public async Task<CreateCommentDto> CreateComment(CreateCommentDto comment)
    {
      try
      {
        var newComment = new CommentModel
        {
          CommentMessage = comment.CommentMessage,
          UserId = comment.UserId,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(newComment);
        await _context.SaveChangesAsync();

        return new CreateCommentDto
        {
          Id = newComment.Id,
          UserId = newComment.UserId,
          CommentMessage = comment.CommentMessage,
          CreatedAt = newComment.CreatedAt,
          UpdatedAt = newComment.UpdatedAt
        };
      }
      catch (Exception ex)
      {
        throw new Exception($"Error creating comment: {ex.Message}");
      }
    }

    public async Task<List<CommentDto>> GetAllComments()
    {
      var comments = await _context.Comments.ToListAsync();
      return comments.Select(c => new CommentDto
      {
        Id = c.Id,
        UserId = c.UserId,
        CommentMessage = c.CommentMessage,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
      }).ToList();
    }

    public async Task<CommentDto> GetCommentById(int id)
    {
      var comment = await _context.Comments.FindAsync(id);

      if (comment == null)
      {
        return null;
      }

      return new CommentDto
      {
        Id = comment.Id,
        UserId = comment.UserId,
        CommentMessage = comment.CommentMessage,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt
      };
    }

    public async Task<CommentDto> UpdateComment(int id, CommentDto comment)
    {
      var existingComment = await _context.Comments.FindAsync(id);

      if (existingComment == null)
      {
        return null;
      }

      existingComment.CommentMessage = comment.CommentMessage;
      existingComment.UpdatedAt = DateTime.UtcNow;

      _context.Comments.Update(existingComment);
      await _context.SaveChangesAsync();

      return new CommentDto
      {
        Id = existingComment.Id,
        UserId = existingComment.UserId,
        CommentMessage = existingComment.CommentMessage,
        CreatedAt = existingComment.CreatedAt,
        UpdatedAt = existingComment.UpdatedAt
      };
    }

    public async Task<CommentDto> DeleteComment(int id)
    {
      var comment = await _context.Comments.FindAsync(id);

      if (comment == null)
      {
        return null;
      }

      _context.Comments.Remove(comment);
      await _context.SaveChangesAsync();

      return new CommentDto
      {
        Id = comment.Id,
        UserId = comment.UserId,
        CommentMessage = comment.CommentMessage,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt
      };
    }
  }
}
