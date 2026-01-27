using Comment.Dto;

namespace Comment.Interfaces
{
  public interface ICommentService
  {
    Task<CreateCommentDto> CreateComment(CreateCommentDto comment);
    Task<List<CommentDto>> GetAllComments();
    Task<CommentDto> GetCommentById(int id);
    Task<CommentDto> UpdateComment(int id, CommentDto comment);
    Task<CommentDto> DeleteComment(int id);
    Task<CommentDto> LikeComment(int id);
    Task<CommentDto> UnlikeComment(int id);
    Task<CommentDto> RetweetComment(int id);
  }
}