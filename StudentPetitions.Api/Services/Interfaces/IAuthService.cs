using StudentPetitions.Api.Models.Auth;

namespace StudentPetitions.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
