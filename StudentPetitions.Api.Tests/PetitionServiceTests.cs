using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Infrastructure.Exceptions;
using StudentPetitions.Api.Infrastructure.Mapping;
using StudentPetitions.Api.Models.Petitions;
using StudentPetitions.Api.Repositories;
using StudentPetitions.Api.Services;

namespace StudentPetitions.Api.Tests;

public class PetitionServiceTests
{
    private readonly Mock<IPetitionRepository> _petitionRepository = new();
    private readonly Mock<IStudentRepository> _studentRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly IMapper _mapper = CreateMapper();

    [Fact]
    public async Task CreateAsync_ShouldCreateDraftPetition_WhenStudentExists()
    {
        Petition? capturedPetition = null;
        var request = new CreatePetitionRequest
        {
            StudentId = Guid.NewGuid(),
            PetitionType = PetitionType.CourseRetake,
            Title = " Course retake request ",
            Description = " Need to retake the course. "
        };

        _studentRepository
            .Setup(repository => repository.GetByIdAsync(request.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Student { Id = request.StudentId });
        _petitionRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Petition>(), It.IsAny<CancellationToken>()))
            .Callback<Petition, CancellationToken>((petition, _) => capturedPetition = petition)
            .Returns(Task.CompletedTask);
        _petitionRepository
            .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(PetitionStatus.Draft, result.Status);
        Assert.NotNull(capturedPetition);
        Assert.Equal(request.StudentId, capturedPetition.StudentId);
        Assert.Equal(request.PetitionType, capturedPetition.PetitionType);
        Assert.Equal("Course retake request", capturedPetition.Title);
        Assert.Equal("Need to retake the course.", capturedPetition.Description);
        Assert.Equal(PetitionStatus.Draft, capturedPetition.Status);
        _petitionRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Petition>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateDraftPetition()
    {
        var petition = CreatePetition(PetitionStatus.Draft);
        var request = new UpdatePetitionRequest
        {
            PetitionType = PetitionType.AcademicLeave,
            Title = " Academic leave ",
            Description = " Need academic leave. "
        };

        _petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);
        _petitionRepository
            .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.UpdateAsync(petition.Id, request);

        Assert.NotNull(result);
        Assert.Equal(PetitionType.AcademicLeave, result.PetitionType);
        Assert.Equal("Academic leave", result.Title);
        Assert.Equal("Need academic leave.", result.Description);
        Assert.Equal(PetitionType.AcademicLeave, petition.PetitionType);
        Assert.Equal("Academic leave", petition.Title);
        Assert.Equal("Need academic leave.", petition.Description);
        Assert.Equal(PetitionStatus.Draft, petition.Status);
        _petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowBusinessRuleException_WhenPetitionIsNotDraft()
    {
        var petition = CreatePetition(PetitionStatus.Submitted);

        _petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);

        var service = CreateService();

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.UpdateAsync(petition.Id, CreateUpdatePetitionRequest()));
        _petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_ShouldChangeStatusToSubmitted_WhenPetitionIsDraft()
    {
        var petition = CreatePetition(PetitionStatus.Draft);

        _petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);
        _petitionRepository
            .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.SubmitAsync(petition.Id);

        Assert.Equal(PetitionStatus.Submitted, result.Status);
        Assert.Equal(PetitionStatus.Submitted, petition.Status);
        _petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReviewAsync_ShouldApproveSubmittedPetition_WhenDecisionIsApproved()
    {
        var petition = CreatePetition(PetitionStatus.Submitted);
        var request = new ReviewPetitionRequest
        {
            Decision = PetitionReviewDecision.Approved,
            ReviewComment = " Approved after review. ",
            ReviewedBy = " Reviewer Admin "
        };

        _petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);
        _petitionRepository
            .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.ReviewAsync(petition.Id, request);

        Assert.Equal(PetitionStatus.Approved, result.Status);
        Assert.Equal(PetitionStatus.Approved, petition.Status);
        Assert.Equal("Approved after review.", petition.ReviewComment);
        Assert.Equal("Reviewer Admin", petition.ReviewedBy);
        Assert.NotNull(petition.ReviewedAt);
        _petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReviewAsync_ShouldThrowBusinessRuleException_WhenPetitionIsNotSubmitted()
    {
        var petition = CreatePetition(PetitionStatus.Draft);

        _petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);

        var service = CreateService();

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.ReviewAsync(petition.Id, CreateReviewPetitionRequest()));
        _petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private PetitionService CreateService()
    {
        return new PetitionService(
            _petitionRepository.Object,
            _studentRepository.Object,
            _currentUserService.Object,
            _mapper);
    }

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            configurationExpression => configurationExpression.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance);

        return configuration.CreateMapper();
    }

    private static UpdatePetitionRequest CreateUpdatePetitionRequest()
    {
        return new UpdatePetitionRequest
        {
            PetitionType = PetitionType.AcademicLeave,
            Title = "Academic leave",
            Description = "Need academic leave."
        };
    }

    private static ReviewPetitionRequest CreateReviewPetitionRequest()
    {
        return new ReviewPetitionRequest
        {
            Decision = PetitionReviewDecision.Approved,
            ReviewComment = "Approved after review.",
            ReviewedBy = "Reviewer Admin"
        };
    }

    private static Petition CreatePetition(PetitionStatus status)
    {
        return new Petition
        {
            Id = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            PetitionType = PetitionType.CourseRetake,
            Title = "Course retake request",
            Description = "Need to retake the course.",
            Status = status
        };
    }
}
