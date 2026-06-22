using Microsoft.EntityFrameworkCore;
using StudentPetitions.Api.Data;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Models.Petitions;
using StudentPetitions.Api.Repositories.Interfaces;

namespace StudentPetitions.Api.Repositories.Implementations;

public class PetitionRepository(AppDbContext dbContext) : IPetitionRepository
{
    public async Task<Petition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Petitions
            .AsNoTracking()
            .FirstOrDefaultAsync(petition => petition.Id == id, cancellationToken);
    }

    public async Task<Petition?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Petitions
            .FirstOrDefaultAsync(petition => petition.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Petition>> GetPagedAsync(
        PetitionFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(dbContext.Petitions.AsNoTracking(), filter)
            .OrderByDescending(petition => petition.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(PetitionFilterRequest filter, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(dbContext.Petitions.AsNoTracking(), filter)
            .CountAsync(cancellationToken);
    }

    public async Task AddAsync(Petition petition, CancellationToken cancellationToken = default)
    {
        await dbContext.Petitions.AddAsync(petition, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Petition> ApplyFilters(IQueryable<Petition> query, PetitionFilterRequest filter)
    {
        if (filter.Status.HasValue)
        {
            query = query.Where(petition => petition.Status == filter.Status.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(petition => petition.PetitionType == filter.Type.Value);
        }

        if (filter.StudentId.HasValue)
        {
            query = query.Where(petition => petition.StudentId == filter.StudentId.Value);
        }

        if (filter.DateFrom.HasValue)
        {
            query = query.Where(petition => petition.CreatedAt >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(petition => petition.CreatedAt <= filter.DateTo.Value);
        }

        return query;
    }
}
