public record UserDto(string username, string password);

public record UserModel()
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}