using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Models.Petitions;

public sealed class PetitionFilterRequest
{
    public const int MaxPageSize = 100;

    private int _pageNumber = 1;
    private int _pageSize = 10;

    public PetitionStatus? Status { get; set; }

    public PetitionType? Type { get; set; }

    public Guid? StudentId { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = Math.Max(1, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
}
