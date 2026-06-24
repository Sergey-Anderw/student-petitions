using Microsoft.IdentityModel.Tokens;
using StudentPetitions.Api.Infrastructure.Auth;
using StudentPetitions.Api.Infrastructure.Exceptions;
using StudentPetitions.Api.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentPetitions.Api.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public class AuthService(IConfiguration configuration) : IAuthService
{
    public Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = GetAuthenticatedUser(request);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(GetExpirationMinutes());
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(ClaimTypes.NameIdentifier, user.Username),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        if (user.StudentId.HasValue)
        {
            claims.Add(new Claim("studentId", user.StudentId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetRequiredSetting("Jwt:Key")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: GetRequiredSetting("Jwt:Issuer"),
            audience: GetRequiredSetting("Jwt:Audience"),
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return Task.FromResult(new LoginResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            Role = user.Role
        });
    }

    private AuthenticatedUser? GetAuthenticatedUser(LoginRequest request)
    {
        if (request is { Username: DemoUsers.StudentUsername, Password: DemoUsers.StudentPassword })
        {
            return new AuthenticatedUser
            {
                Username = DemoUsers.StudentUsername,
                Role = DemoUsers.StudentRole,
                StudentId = DemoUsers.StudentId
            };
        }

        if (request is { Username: DemoUsers.ReviewerUsername, Password: DemoUsers.ReviewerPassword })
        {
            return new AuthenticatedUser
            {
                Username = DemoUsers.ReviewerUsername,
                Role = DemoUsers.ReviewerRole
            };
        }

        return null;
    }

    private int GetExpirationMinutes()
    {
        return int.TryParse(configuration["Jwt:ExpirationMinutes"], out var minutes)
            ? minutes
            : 60;
    }

    private string GetRequiredSetting(string key)
    {
        return configuration[key]
            ?? throw new InvalidOperationException($"Missing required configuration value: {key}");
    }
}
