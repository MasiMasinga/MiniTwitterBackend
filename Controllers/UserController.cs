using User.Dto;
using User.Interfaces;
using Tweet.Data;
using Microsoft.EntityFrameworkCore;
using MiniTwitterBackend.Helpers.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace User.Controller
{
  [ApiController]
  [Route("api/user")]
  [Authorize]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;
    private readonly AppDbContext _context;

    public UserController(IUserService userService, AppDbContext context)
    {
      _userService = userService;
      _context = context;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<CreateUserDto>> RegisterUser([FromBody] CreateUserDto? user)
    {
      if (user == null)
      {
        return BadRequest(new { message = "Request body is required." });
      }

      try
      {
        await UserValidation.ValidateRegistrationAsync(_context, user);

        var result = await _userService.RegisterUser(user);

        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<CreateUserDto>> LoginUser([FromBody] LoginUserDto user)
    {
      if (user == null)
      {
        return BadRequest(new { message = "Request body is required." });
      }

      try
      {
        var result = await _userService.LoginUser(user);

        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPost("google-auth")]
    [AllowAnonymous]
    public async Task<ActionResult<CreateUserDto>> GoogleAuth([FromBody] GoogleAuthDto request)
    {
      var result = await _userService.GoogleAuth(request);

      return Ok(result);
    }

    [HttpGet(template: "user-details")]
    public async Task<ActionResult<UserDto>> GetUserDetails(int id)
    {
      var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized();
      }

      var result = await _userService.GetUserDetails(userId);

      return Ok(result);
    }

    [HttpPut("update-user-details/{id}")]
    public async Task<ActionResult<UserDto>> UpdateUserDetails([FromBody] UserDto? user, int id)
    {
      var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized();
      }

      if (id != userId)
      {
        return Forbid();
      }

      if (user == null)
      {
        return BadRequest(new { message = "Request body is required." });
      }

      if (!await _context.User.AnyAsync(u => u.Id == userId))
      {
        return NotFound(new { message = "User not found." });
      }

      try
      {
        await UserValidation.ValidateUpdateUserDetailsAsync(_context, user, userId);

        var result = await _userService.UpdateUserDetails(user, userId);

        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPut("update-user-password/{id}")]
    public async Task<ActionResult<CreateUserDto>> UpdateUserPassword([FromBody] CreateUserDto? user, int id)
    {
      var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized();
      }

      if (id != userId)
      {
        return Forbid();
      }

      if (user == null)
      {
        return BadRequest(new { message = "Request body is required." });
      }

      if (!await _context.User.AnyAsync(u => u.Id == userId))
      {
        return NotFound(new { message = "User not found." });
      }

      try
      {
        UserValidation.ValidateUpdatePassword(user);

        var result = await _userService.UpdateUserPassword(user, userId);

        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpDelete("delete-user/{id}")]
    public async Task<ActionResult<bool>> DeleteUser(int id)
    {
      var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized();
      }

      var result = await _userService.DeleteUser(userId);

      return Ok(result);
    }
  }
}