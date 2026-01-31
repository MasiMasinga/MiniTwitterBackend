using Microsoft.EntityFrameworkCore;
using Tweet.Data;
using User.Dto;

namespace MiniTwitterBackend.Helpers.Validation;

public static class UserValidation
{
  public static async Task ValidateRegistrationAsync(AppDbContext context, CreateUserDto user)
  {
    if (string.IsNullOrWhiteSpace(user.Email))
    {
      throw new Exception("Email cannot be empty.");
    }

    if (!user.Email.Contains("@"))
    {
      throw new Exception("Invalid email format.");
    }

    if (string.IsNullOrWhiteSpace(user.Username))
    {
      throw new Exception("Username cannot be empty.");
    }

    if (user.Username.Length < 3)
    {
      throw new Exception("Username must be at least 3 characters long.");
    }

    if (string.IsNullOrWhiteSpace(user.FirstName))
    {
      throw new Exception("First name cannot be empty.");
    }

    if (string.IsNullOrWhiteSpace(user.LastName))
    {
      throw new Exception("Last name cannot be empty.");
    }

    if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 6)
    {
      throw new Exception("Password must be at least 6 characters long.");
    }

    if (await context.User.AnyAsync(u => u.Email == user.Email))
    {
      throw new Exception("User with this email already exists.");
    }

    if (await context.User.AnyAsync(u => u.Username == user.Username))
    {
      throw new Exception("User with this username already exists.");
    }
  }

  public static void ValidateLogin(LoginUserDto user)
  {
    if (string.IsNullOrWhiteSpace(user.EmailOrUsername) || string.IsNullOrWhiteSpace(user.Password))
    {
      throw new Exception("Email/Username and password cannot be empty.");
    }

    if (user.Password.Length < 6)
    {
      throw new Exception("Password must be at least 6 characters long.");
    }
  }
}
