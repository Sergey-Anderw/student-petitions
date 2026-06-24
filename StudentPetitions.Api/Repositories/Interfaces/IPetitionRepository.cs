using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Models.Petitions;

namespace StudentPetitions.Api.Repositories.Interfaces;

public interface IPetitionRepository
{
    Task<Petition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Petition?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Petition>> GetFilteredAsync(
        PetitionFilterRequest filter,
        CancellationToken cancellationToken = default);

    Task AddAsync(Petition petition, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
