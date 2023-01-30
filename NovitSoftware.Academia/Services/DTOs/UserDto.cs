namespace NovitSoftware.Academia.Services.DTOs;

public class UserRegisterDto : UserDto
{
    public string Role { get; set; } = string.Empty;
}


public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
