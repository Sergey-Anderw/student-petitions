using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Models.Petitions;

public class UpdatePetitionRequest
{
    public PetitionType PetitionType { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
