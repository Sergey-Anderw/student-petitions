using FluentValidation;
using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Models.Petitions;

public class ReviewPetitionRequestValidator : AbstractValidator<ReviewPetitionRequest>
{
    public ReviewPetitionRequestValidator()
    {
        RuleFor(request => request.Status)
            .Must(status => status is PetitionStatus.Approved or PetitionStatus.Rejected)
            .WithMessage("Status must be Approved or Rejected.");

        RuleFor(request => request.ReviewedBy)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.ReviewComment)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
