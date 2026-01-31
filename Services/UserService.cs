using User.Dto;
using Tweet.Data;
using Microsoft.EntityFrameworkCore;
using User.Interfaces;
using Microsoft.AspNetCore.Identity;
using UserModel = User.Models.User;

namespace User.Services
{
  public class UserService : IUserService
  {
    private readonly AppDbContext _context;
    private readonly PasswordHasher<UserModel> _passwordHasher = new();

    public UserService(AppDbContext context)
    {
      _context = context;
    }

    public async Task<CreateUserDto> RegisterUser(CreateUserDto user)
    {
      try
      {
        if (await _context.User.AnyAsync(u => u.Email == user.Email))
        {
          throw new Exception("User with this email already exists.");
        }

        if (await _context.User.AnyAsync(u => u.Username == user.Username))
        {
          throw new Exception("User with this username already exists.");
        }

        if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 6)
        {
          throw new Exception("Password must be at least 6 characters long.");
        }

        if (!user.Email.Contains("@"))
        {
          throw new Exception("Invalid email format.");
        }

        if (string.IsNullOrEmpty(user.Username) || user.Username.Length < 3)
        {
          throw new Exception("Username must be at least 3 characters long.");
        }

        if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
        {
          throw new Exception("First name and last name cannot be empty.");
        }

        if (string.IsNullOrEmpty(user.Email))
        {
          throw new Exception("Email cannot be empty.");
        }

        if (string.IsNullOrEmpty(user.Username))
        {
          throw new Exception("Username cannot be empty.");
        }

        if (string.IsNullOrEmpty(user.FirstName))
        {
          throw new Exception("First name cannot be empty.");
        }

        if (string.IsNullOrEmpty(user.LastName))
        {
          throw new Exception("Last name cannot be empty.");
        }

        var newUser = new UserModel
        {
          Username = user.Username,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Email = user.Email,
          Password = string.Empty,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        };

        newUser.Password = _passwordHasher.HashPassword(newUser, user.Password);

        _context.User.Add(newUser);
        await _context.SaveChangesAsync();

        return new CreateUserDto
        {
          Id = newUser.Id,
          Username = newUser.Username,
          FirstName = newUser.FirstName,
          LastName = newUser.LastName,
          Email = newUser.Email
        };
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<CreateUserDto> LoginUser(CreateUserDto user)
    {
      try
      {
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
        {
          throw new Exception("Email and password cannot be empty.");
        }

        if (!user.Email.Contains("@"))
        {
          throw new Exception("Invalid email format.");
        }

        if (user.Password.Length < 6)
        {
          throw new Exception("Password must be at least 6 characters long.");
        }

        var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == user.Email);

        if (existingUser == null)
        {
          throw new Exception("Invalid email or password.");
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.Password, user.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
          throw new Exception("Invalid email or password.");
        }

        return new CreateUserDto
        {
          Id = existingUser.Id,
          Username = existingUser.Username,
          FirstName = existingUser.FirstName,
          LastName = existingUser.LastName,
          Email = existingUser.Email
        };
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<CreateUserDto> GoogleAuth(CreateUserDto user)
    {
      try
      {
        throw new NotImplementedException("GoogleAuth is not implemented yet.");
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<UserDto> GetUserDetails(int id)
    {
      try
      {
        var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
        if (existingUser == null)
        {
          throw new Exception("User not found.");
        }

        return new UserDto
        {
          Id = existingUser.Id,
          Username = existingUser.Username,
          FirstName = existingUser.FirstName,
          LastName = existingUser.LastName,
          Email = existingUser.Email
        };
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<UserDto> UpdateUserDetails(UserDto user, int id)
    {
      try
      {
        if (string.IsNullOrEmpty(user.Username) || user.Username.Length < 3)
        {
          throw new Exception("Username must be at least 3 characters long.");
        }

        if (string.IsNullOrEmpty(user.FirstName))
        {
          throw new Exception("First name cannot be empty.");
        }

        if (string.IsNullOrEmpty(user.LastName))
        {
          throw new Exception("Last name cannot be empty.");
        }

        if (string.IsNullOrEmpty(user.Email) || !user.Email.Contains("@"))
        {
          throw new Exception("Invalid email format.");
        }

        if (await _context.User.AnyAsync(u => u.Email == user.Email && u.Id != id))
        {
          throw new Exception("Another user with this email already exists.");
        }

        if (string.IsNullOrEmpty(user.Email))
        {
          throw new Exception("Email cannot be empty.");
        }

        if (string.IsNullOrEmpty(user.Username))
        {
          throw new Exception("Username cannot be empty.");
        }

        var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
        if (existingUser == null)
        {
          throw new Exception("User not found.");
        }

        existingUser.Username = user.Username;
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.UpdatedAt = DateTime.UtcNow;

        _context.User.Update(existingUser);
        await _context.SaveChangesAsync();

        return new UserDto
        {
          Id = existingUser.Id,
          Username = existingUser.Username,
          FirstName = existingUser.FirstName,
          LastName = existingUser.LastName,
          Email = existingUser.Email
        };
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<CreateUserDto> UpdateUserPassword(CreateUserDto user, int id)
    {
      try
      {
        if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 6)
        {
          throw new Exception("Password must be at least 6 characters long.");
        }

        if (!user.Password.Contains(" "))
        {
          throw new Exception("Password cannot contain spaces.");
        }

        var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
        if (existingUser == null)
        {
          throw new Exception("User not found.");
        }

        existingUser.Password = _passwordHasher.HashPassword(existingUser, user.Password);
        existingUser.UpdatedAt = DateTime.UtcNow;

        _context.User.Update(existingUser);
        await _context.SaveChangesAsync();

        return new CreateUserDto
        {
          Id = existingUser.Id,
          Username = existingUser.Username,
          FirstName = existingUser.FirstName,
          LastName = existingUser.LastName,
          Email = existingUser.Email
        };
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteUser(int id)
    {
      try
      {
        var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
        
        _context.User.Remove(existingUser);
        await _context.SaveChangesAsync();

        return true;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}