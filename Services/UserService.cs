using User.Dto;
using Tweet.Data;
using Microsoft.EntityFrameworkCore;
using MiniTwitterBackend.Helpers.Auth;
using MiniTwitterBackend.Helpers.Validation;
using User.Interfaces;
using Microsoft.AspNetCore.Identity;
using Google.Apis.Auth;
using UserModel = User.Models.User;

namespace User.Services
{
  public class UserService : IUserService
  {
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<UserModel> _passwordHasher = new();

    public UserService(AppDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public async Task<CreateUserDto> RegisterUser(CreateUserDto user)
    {
      try
      {
        await UserValidation.ValidateRegistrationAsync(_context, user);

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

    public async Task<CreateUserDto> LoginUser(LoginUserDto user)
    {
      try
      {
        var identifier = user.EmailOrUsername.Trim();
        var normalized = identifier.ToLowerInvariant();

        var existingUser = await _context.User.FirstOrDefaultAsync(u =>
          u.Email.ToLower() == normalized || u.Username.ToLower() == normalized);

        if (existingUser == null)
        {
          throw new Exception("Invalid email/username or password.");
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.Password, user.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
          throw new Exception("Invalid email/username or password.");
        }

        var accessToken = JwtTokenHelper.GenerateJwtAccessToken(_configuration, existingUser);

        return new CreateUserDto
        {
          Id = existingUser.Id,
          Username = existingUser.Username,
          FirstName = existingUser.FirstName,
          LastName = existingUser.LastName,
          Email = existingUser.Email,
          Access = accessToken
        };
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<CreateUserDto> GoogleAuth(GoogleAuthDto request)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(request.IdToken))
        {
          throw new Exception("Google ID token is required.");
        }

        var googleClientId = _configuration["Authentication:Google:ClientId"];
        if (string.IsNullOrWhiteSpace(googleClientId))
        {
          throw new Exception(
            "Google ClientId is not configured. Please set Authentication:Google:ClientId in appsettings.");
        }

        var payload = await GoogleJsonWebSignature.ValidateAsync(
          request.IdToken,
          new GoogleJsonWebSignature.ValidationSettings
          {
            Audience = new[] { googleClientId }
          }
        );

        if (payload == null)
        {
          throw new Exception("Invalid Google token.");
        }

        if (payload.EmailVerified != true)
        {
          throw new Exception("Google email is not verified.");
        }

        if (string.IsNullOrWhiteSpace(payload.Email))
        {
          throw new Exception("Google token did not include an email.");
        }

        var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == payload.Email);
        if (existingUser != null)
        {
          var accessToken = JwtTokenHelper.GenerateJwtAccessToken(_configuration, existingUser);
          return new CreateUserDto
          {
            Id = existingUser.Id,
            Username = existingUser.Username,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
            Email = existingUser.Email,
            Access = accessToken
          };
        }

        var baseUsername = payload.Email.Split('@')[0].Trim();
        if (string.IsNullOrWhiteSpace(baseUsername))
        {
          baseUsername = "user";
        }

        var username = baseUsername.Length > 100 ? baseUsername[..100] : baseUsername;
        var suffix = 0;
        while (await _context.User.AnyAsync(u => u.Username == username))
        {
          suffix++;
          var candidate = $"{baseUsername}{suffix}";
          username = candidate.Length > 100 ? candidate[..100] : candidate;
          if (suffix > 10_000)
          {
            throw new Exception("Could not generate a unique username.");
          }
        }

        var newUser = new UserModel
        {
          Username = username,
          FirstName = string.IsNullOrWhiteSpace(payload.GivenName) ? "Google" : payload.GivenName,
          LastName = string.IsNullOrWhiteSpace(payload.FamilyName) ? "User" : payload.FamilyName,
          Email = payload.Email,
          Password = string.Empty,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        };

        var randomSecret = Guid.NewGuid().ToString("N");
        newUser.Password = _passwordHasher.HashPassword(newUser, randomSecret);

        _context.User.Add(newUser);
        await _context.SaveChangesAsync();

        var newAccessToken = JwtTokenHelper.GenerateJwtAccessToken(_configuration, newUser);
        return new CreateUserDto
        {
          Id = newUser.Id,
          Username = newUser.Username,
          FirstName = newUser.FirstName,
          LastName = newUser.LastName,
          Email = newUser.Email,
          Access = newAccessToken
        };
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