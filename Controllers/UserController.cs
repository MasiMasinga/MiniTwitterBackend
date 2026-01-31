using User.Dto;
using User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace User.Controller
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterUser([FromBody] CreateUserDto user)
    {
      var result = await _userService.RegisterUser(user);

      return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> LoginUser([FromBody] CreateUserDto user)
    {
      var result = await _userService.LoginUser(user);
        
      return Ok(result);
    }

    [HttpPost("google-auth")]
    public async Task<ActionResult<CreateUserDto>> GoogleAuth([FromBody] GoogleAuthDto request)
    {
      var result = await _userService.GoogleAuth(request);
        
      return Ok(result);
    }

    [HttpGet(template: "user-details")]
    public async Task<ActionResult<UserDto>> GetUserDetails(int id)
    {
      var result = await _userService.GetUserDetails(id);
        
      return Ok(result);
    }

    [HttpPut("update-user-details/{id}")]
    public async Task<ActionResult<UserDto>> UpdateUserDetails([FromBody] UserDto user, int id)
    {
      var result = await _userService.UpdateUserDetails(user, id);
        
      return Ok(result);
    }

    [HttpPut("update-user-password/{id}")]
    public async Task<ActionResult<CreateUserDto>> UpdateUserPassword([FromBody] CreateUserDto user, int id)
    {
      var result = await _userService.UpdateUserPassword(user, id);
        
      return Ok(result);
    } 

    [HttpDelete("delete-user/{id}")]
    public async Task<ActionResult<bool>> DeleteUser(int id)
    {
      var result = await _userService.DeleteUser(id);
        
      return Ok(result);
    }
  }
}