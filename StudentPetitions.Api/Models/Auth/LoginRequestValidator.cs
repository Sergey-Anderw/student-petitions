using FluentValidation;

namespace StudentPetitions.Api.Models.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty();

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}
