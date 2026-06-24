using FluentValidation;

namespace StudentPetitions.Api.Models.Petitions;

public sealed class PetitionFilterRequestValidator : AbstractValidator<PetitionFilterRequest>
{
    public PetitionFilterRequestValidator()
    {
        RuleFor(request => request.DateTo)
            .GreaterThanOrEqualTo(request => request.DateFrom)
            .When(request => request.DateFrom.HasValue && request.DateTo.HasValue);
    }
}
