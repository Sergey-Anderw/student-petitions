namespace StudentPetitions.Api.Models.Students;

public class StudentResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string StudentNumber { get; set; } = string.Empty;

    public string CreatedAt { get; set; } = string.Empty;
}
