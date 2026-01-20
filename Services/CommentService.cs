using Comment.Dto;
using Tweet.Data;
using Comment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Comment.Services
{
  public class CommentService : ICommentService
  {
    private readonly AppDbContext _context;

    public CommentService(AppDbContext context)
    {
      _context = context;
    }
  }
}
