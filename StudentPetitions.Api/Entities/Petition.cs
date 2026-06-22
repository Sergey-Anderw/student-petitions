namespace StudentPetitions.Api.Entities;

public class Petition
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Student Student { get; set; } = null!;

    public PetitionType PetitionType { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public PetitionStatus Status { get; set; } = PetitionStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewComment { get; set; }
}
