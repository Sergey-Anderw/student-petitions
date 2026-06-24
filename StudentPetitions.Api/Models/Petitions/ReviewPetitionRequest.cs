namespace StudentPetitions.Api.Models.Petitions;

public class ReviewPetitionRequest
{
    public PetitionReviewDecision Decision { get; set; }

    public string ReviewedBy { get; set; } = string.Empty;

    public string ReviewComment { get; set; } = string.Empty;
}
