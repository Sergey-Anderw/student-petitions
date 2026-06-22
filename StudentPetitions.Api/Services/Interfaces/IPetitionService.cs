using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Petitions;

namespace StudentPetitions.Api.Services.Interfaces;

public interface IPetitionService
{
    Task<CreatePetitionResult> CreateAsync(
        CreatePetitionRequest request,
        CancellationToken cancellationToken = default);

    Task<PetitionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResponse<PetitionResponse>> GetPagedAsync(
        PetitionFilterRequest filter,
        CancellationToken cancellationToken = default);

    Task<UpdatePetitionResult> UpdateAsync(
        Guid id,
        UpdatePetitionRequest request,
        CancellationToken cancellationToken = default);
}
