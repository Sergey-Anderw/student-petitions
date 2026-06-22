using FluentValidation;

namespace StudentPetitions.Api.Models.Petitions;

public class CreatePetitionRequestValidator : AbstractValidator<CreatePetitionRequest>
{
    public CreatePetitionRequestValidator()
    {
        RuleFor(request => request.StudentId)
            .NotEmpty();

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
