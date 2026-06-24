using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Models.Petitions;

public sealed record UpdatePetitionRequest
{
    public PetitionType PetitionType { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
