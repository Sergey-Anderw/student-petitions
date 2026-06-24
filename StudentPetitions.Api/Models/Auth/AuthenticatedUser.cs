namespace StudentPetitions.Api.Models.Auth;

public class AuthenticatedUser
{
    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public Guid? StudentId { get; set; }
}
