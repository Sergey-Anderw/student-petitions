namespace StudentPetitions.Api.Services.Interfaces;

public interface ICurrentUserService
{
    string? UserName { get; }

    string? Role { get; }

    Guid? StudentId { get; }

    bool IsStudent { get; }

    bool IsReviewer { get; }
}
