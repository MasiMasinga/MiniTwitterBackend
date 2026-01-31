using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserModel = User.Models.User;

namespace MiniTwitterBackend.Helpers.Auth;

public static class JwtTokenHelper
{
  public static string GenerateJwtAccessToken(IConfiguration configuration, UserModel user)
  {
    var issuer = configuration["Jwt:Issuer"];
    var audience = configuration["Jwt:Audience"];
    var key = configuration["Jwt:Key"];
    var expiresMinutesStr = configuration["Jwt:ExpiresMinutes"];

    if (string.IsNullOrWhiteSpace(key))
    {
      throw new Exception("JWT key is not configured (Jwt:Key).");
    }

    var expiresMinutes = 120;
    if (int.TryParse(expiresMinutesStr, out var parsed) && parsed > 0)
    {
      expiresMinutes = parsed;
    }

    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Name, user.Username),
      new(ClaimTypes.Email, user.Email),
      new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new(JwtRegisteredClaimNames.Email, user.Email),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: string.IsNullOrWhiteSpace(issuer) ? null : issuer,
      audience: string.IsNullOrWhiteSpace(audience) ? null : audience,
      claims: claims,
      notBefore: DateTime.UtcNow,
      expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}

