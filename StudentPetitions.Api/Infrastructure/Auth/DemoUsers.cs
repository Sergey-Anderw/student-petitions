namespace StudentPetitions.Api.Infrastructure.Auth;

public static class DemoUsers
{
    public static readonly Guid StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public const string StudentUsername = "student";

    public const string StudentPassword = "student123";

    public const string StudentRole = "Student";

    public const string ReviewerUsername = "reviewer";

    public const string ReviewerPassword = "reviewer123";

    public const string ReviewerRole = "Reviewer";
}
