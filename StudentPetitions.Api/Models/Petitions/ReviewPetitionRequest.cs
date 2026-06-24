using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Models.Petitions;

public class ReviewPetitionRequest
{
    public PetitionStatus Status { get; set; }

    public string ReviewedBy { get; set; } = string.Empty;

    public string ReviewComment { get; set; } = string.Empty;
}
