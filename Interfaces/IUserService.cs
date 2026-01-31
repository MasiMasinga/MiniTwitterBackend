using User.Dto;

namespace User.Interfaces
{
  public interface IUserService
  {
    Task<CreateUserDto> RegisterUser(CreateUserDto user);
    Task<CreateUserDto> LoginUser(CreateUserDto user);
    Task<CreateUserDto> GoogleAuth(GoogleAuthDto request);
    Task<UserDto> GetUserDetails(int id);
    Task<UserDto> UpdateUserDetails(UserDto user, int id);
    Task<CreateUserDto> UpdateUserPassword(CreateUserDto user, int id);
    Task<bool> DeleteUser(int id);
  }
}