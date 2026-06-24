using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Models.Petitions;

public sealed record PetitionFilterRequest
{
    public PetitionStatus? Status { get; set; }

    public PetitionType? Type { get; set; }

    public Guid? StudentId { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }
}
