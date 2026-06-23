using AutoMapper;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Petitions;
using StudentPetitions.Api.Repositories.Interfaces;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Services.Implementations;

public class PetitionService(
    IPetitionRepository petitionRepository,
    IStudentRepository studentRepository,
    IMapper mapper)
    : IPetitionService
{
    public async Task<Result<PetitionResponse>> CreateAsync(
        CreatePetitionRequest request,
        CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken);

        if (student is null)
        {
            return Result<PetitionResponse>.NotFound("Student was not found.");
        }

        request.Title = request.Title.Trim();
        request.Description = request.Description.Trim();

        var petition = mapper.Map<Petition>(request);
        petition.Status = PetitionStatus.Draft;

        await petitionRepository.AddAsync(petition, cancellationToken);
        await petitionRepository.SaveChangesAsync(cancellationToken);

        return Result<PetitionResponse>.Success(mapper.Map<PetitionResponse>(petition));
    }

    public async Task<PetitionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var petition = await petitionRepository.GetByIdAsync(id, cancellationToken);

        return petition is null ? null : mapper.Map<PetitionResponse>(petition);
    }

    public async Task<PagedResponse<PetitionResponse>> GetPagedAsync(
        PetitionFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var petitions = await petitionRepository.GetPagedAsync(filter, cancellationToken);
        var totalCount = await petitionRepository.CountAsync(filter, cancellationToken);
        var responses = mapper.Map<IReadOnlyCollection<PetitionResponse>>(petitions);

        return new PagedResponse<PetitionResponse>(
            responses,
            filter.PageNumber,
            filter.PageSize,
            totalCount);
    }

    public async Task<Result<PetitionResponse>> UpdateAsync(
        Guid id,
        UpdatePetitionRequest request,
        CancellationToken cancellationToken = default)
    {
        var petition = await petitionRepository.GetTrackedByIdAsync(id, cancellationToken);

        if (petition is null)
        {
            return Result<PetitionResponse>.NotFound("Petition was not found.");
        }

        if (petition.Status != PetitionStatus.Draft)
        {
            return Result<PetitionResponse>.Conflict("Only draft petitions can be updated.");
        }

        petition.PetitionType = request.PetitionType;
        petition.Title = request.Title.Trim();
        petition.Description = request.Description.Trim();
        petition.UpdatedAt = DateTime.UtcNow;

        await petitionRepository.SaveChangesAsync(cancellationToken);

        return Result<PetitionResponse>.Success(mapper.Map<PetitionResponse>(petition));
    }
}
