using FluentValidation;

namespace StudentPetitions.Api.Models.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty();

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}
