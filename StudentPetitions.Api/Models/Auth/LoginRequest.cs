namespace StudentPetitions.Api.Models.Auth;

public sealed record LoginRequest
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
