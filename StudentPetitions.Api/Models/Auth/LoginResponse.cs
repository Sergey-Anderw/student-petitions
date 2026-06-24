namespace StudentPetitions.Api.Models.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public DateTime ExpiresAt { get; set; }

    public string Role { get; set; } = string.Empty;
}
