using FluentValidation;

namespace StudentPetitions.Api.Models.Petitions;

public sealed class ReviewPetitionRequestValidator : AbstractValidator<ReviewPetitionRequest>
{
    public ReviewPetitionRequestValidator()
    {
        RuleFor(request => request.Decision)
            .IsInEnum();

        RuleFor(request => request.ReviewedBy)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.ReviewComment)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
