using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Models.Petitions;

public class PetitionResponse
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public PetitionType PetitionType { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public PetitionStatus Status { get; set; }

    public string CreatedAt { get; set; } = string.Empty;

    public string UpdatedAt { get; set; } = string.Empty;

    public string? ReviewedBy { get; set; }

    public string? ReviewedAt { get; set; }

    public string? ReviewComment { get; set; }
}
