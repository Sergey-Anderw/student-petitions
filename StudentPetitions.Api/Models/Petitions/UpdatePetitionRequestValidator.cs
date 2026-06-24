using FluentValidation;

namespace StudentPetitions.Api.Models.Petitions;

public sealed class UpdatePetitionRequestValidator : AbstractValidator<UpdatePetitionRequest>
{
    public UpdatePetitionRequestValidator()
    {
        RuleFor(request => request.PetitionType)
            .IsInEnum();

        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
