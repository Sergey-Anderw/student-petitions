using AutoMapper;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Infrastructure.Exceptions;
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
    public async Task<PetitionResponse> CreateAsync(
        CreatePetitionRequest request,
        CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken);

        if (student is null)
        {
            throw new NotFoundException("Student was not found.");
        }

        request.Title = request.Title.Trim();
        request.Description = request.Description.Trim();

        var petition = mapper.Map<Petition>(request);
        petition.Status = PetitionStatus.Draft;

        await petitionRepository.AddAsync(petition, cancellationToken);
        await petitionRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<PetitionResponse>(petition);
    }

    public async Task<PetitionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var petition = await petitionRepository.GetByIdAsync(id, cancellationToken);

        if (petition is null)
        {
            throw new NotFoundException("Petition was not found.");
        }

        return mapper.Map<PetitionResponse>(petition);
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

    public async Task<PetitionResponse> UpdateAsync(
        Guid id,
        UpdatePetitionRequest request,
        CancellationToken cancellationToken = default)
    {
        var petition = await petitionRepository.GetTrackedByIdAsync(id, cancellationToken);

        if (petition is null)
        {
            throw new NotFoundException("Petition was not found.");
        }

        if (petition.Status != PetitionStatus.Draft)
        {
            throw new BusinessRuleException("Only draft petitions can be updated.");
        }

        petition.PetitionType = request.PetitionType;
        petition.Title = request.Title.Trim();
        petition.Description = request.Description.Trim();
        petition.UpdatedAt = DateTime.UtcNow;

        await petitionRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<PetitionResponse>(petition);
    }

    public async Task<PetitionResponse> SubmitAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var petition = await petitionRepository.GetTrackedByIdAsync(id, cancellationToken);

        if (petition is null)
        {
            throw new NotFoundException("Petition was not found.");
        }

        if (petition.Status != PetitionStatus.Draft)
        {
            throw new BusinessRuleException("Only draft petitions can be submitted.");
        }

        petition.Status = PetitionStatus.Submitted;
        petition.UpdatedAt = DateTime.UtcNow;

        await petitionRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<PetitionResponse>(petition);
    }

    public async Task<PetitionResponse> ReviewAsync(
        Guid id,
        ReviewPetitionRequest request,
        CancellationToken cancellationToken = default)
    {
        var petition = await petitionRepository.GetTrackedByIdAsync(id, cancellationToken);

        if (petition is null)
        {
            throw new NotFoundException("Petition was not found.");
        }

        if (petition.Status != PetitionStatus.Submitted)
        {
            throw new BusinessRuleException("Only submitted petitions can be reviewed.");
        }

        var now = DateTime.UtcNow;

        if (petition.Status == PetitionStatus.Submitted)
        {
            petition.Status = PetitionStatus.UnderReview;
        }

        petition.Status = request.Status;
        petition.ReviewedBy = request.ReviewedBy.Trim();
        petition.ReviewComment = request.ReviewComment.Trim();
        petition.ReviewedAt = now;
        petition.UpdatedAt = now;

        await petitionRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<PetitionResponse>(petition);
    }
}
