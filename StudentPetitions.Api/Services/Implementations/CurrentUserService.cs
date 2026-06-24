using System.Security.Claims;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Services.Implementations;

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
