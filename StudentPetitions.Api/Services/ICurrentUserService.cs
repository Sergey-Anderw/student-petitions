using System.Security.Claims;

namespace StudentPetitions.Api.Services;

public interface ICurrentUserService
{
    string? UserName { get; }

    string? Role { get; }

    Guid? StudentId { get; }

    bool IsStudent { get; }

    bool IsReviewer { get; }
}


public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? UserName => User?.Identity?.Name;

    public string? Role => User?.FindFirstValue(ClaimTypes.Role);

    public Guid? StudentId
    {
        get
        {
            var value = User?.FindFirstValue("studentId");

            return Guid.TryParse(value, out var studentId)
                ? studentId
                : null;
        }
    }

    public bool IsStudent => Role == "Student";

    public bool IsReviewer => Role == "Reviewer";
}
