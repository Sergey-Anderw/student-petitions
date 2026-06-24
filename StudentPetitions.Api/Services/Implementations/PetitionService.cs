using AutoMapper;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Infrastructure.Exceptions;
using StudentPetitions.Api.Models.Petitions;
using StudentPetitions.Api.Repositories.Interfaces;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Services.Implementations;

public class PetitionService(
    IPetitionRepository petitionRepository,
    IStudentRepository studentRepository,
    ICurrentUserService currentUserService,
    IMapper mapper)
    : IPetitionService
{
    public async Task<PetitionResponse> CreateAsync(
        CreatePetitionRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureStudentCanAccessStudent(request.StudentId, "Students can create petitions only for themselves.");

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

        EnsureStudentCanAccessPetition(petition);

        return mapper.Map<PetitionResponse>(petition);
    }

    public async Task<IReadOnlyCollection<PetitionResponse>> GetFilteredAsync(
        PetitionFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        if (currentUserService.IsStudent)
        {
            filter.StudentId = currentUserService.StudentId;
        }

        var petitions = await petitionRepository.GetFilteredAsync(filter, cancellationToken);

        return mapper.Map<IReadOnlyCollection<PetitionResponse>>(petitions);
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

        EnsureStudentCanAccessPetition(petition);

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

        EnsureStudentCanAccessPetition(petition);

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

        petition.Status = request.Decision switch
        {
            PetitionReviewDecision.Approved => PetitionStatus.Approved,
            PetitionReviewDecision.Rejected => PetitionStatus.Rejected,
            _ => throw new BusinessRuleException("Review decision must be Approved or Rejected.")
        };
        petition.ReviewedBy = request.ReviewedBy.Trim();
        petition.ReviewComment = request.ReviewComment.Trim();
        petition.ReviewedAt = now;
        petition.UpdatedAt = now;

        await petitionRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<PetitionResponse>(petition);
    }

    private void EnsureStudentCanAccessStudent(Guid studentId, string errorMessage)
    {
        if (!currentUserService.IsStudent)
        {
            return;
        }

        if (currentUserService.StudentId != studentId)
        {
            throw new BusinessRuleException(errorMessage);
        }
    }

    private void EnsureStudentCanAccessPetition(Petition petition)
    {
        if (!currentUserService.IsStudent)
        {
            return;
        }

        if (currentUserService.StudentId != petition.StudentId)
        {
            throw new NotFoundException("Petition was not found.");
        }
    }
}
