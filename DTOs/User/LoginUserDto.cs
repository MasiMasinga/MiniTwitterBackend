namespace User.Dto
{
  public class LoginUserDto
  {
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Access { get; set; } = string.Empty;
  }
}

