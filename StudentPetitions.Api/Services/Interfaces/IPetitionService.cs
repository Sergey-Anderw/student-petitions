using StudentPetitions.Api.Models.Petitions;

namespace StudentPetitions.Api.Services.Interfaces;

public interface IPetitionService
{
    Task<PetitionResponse> CreateAsync(
        CreatePetitionRequest request,
        CancellationToken cancellationToken = default);

    Task<PetitionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PetitionResponse>> GetFilteredAsync(
        PetitionFilterRequest filter,
        CancellationToken cancellationToken = default);

    Task<PetitionResponse> UpdateAsync(
        Guid id,
        UpdatePetitionRequest request,
        CancellationToken cancellationToken = default);

    Task<PetitionResponse> SubmitAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PetitionResponse> ReviewAsync(
        Guid id,
        ReviewPetitionRequest request,
        CancellationToken cancellationToken = default);
}
