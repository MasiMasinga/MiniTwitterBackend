using User.Dto;
using User.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace User.Controller
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<CreateUserDto>> RegisterUser([FromBody] CreateUserDto user)
    {
      var result = await _userService.RegisterUser(user);

      return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<CreateUserDto>> LoginUser([FromBody] LoginUserDto user)
    {
      var result = await _userService.LoginUser(user);
        
      return Ok(result);
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
    public async Task<ActionResult<UserDto>> UpdateUserDetails([FromBody] UserDto user, int id)
    {
      var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized();
      }

      var result = await _userService.UpdateUserDetails(user, userId);
        
      return Ok(result);
    }

    [HttpPut("update-user-password/{id}")]
    public async Task<ActionResult<CreateUserDto>> UpdateUserPassword([FromBody] CreateUserDto user, int id)
    {
      var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized();
      }

      var result = await _userService.UpdateUserPassword(user, userId);
        
      return Ok(result);
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