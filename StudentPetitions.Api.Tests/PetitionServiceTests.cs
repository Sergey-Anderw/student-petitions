using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Infrastructure.Exceptions;
using StudentPetitions.Api.Infrastructure.Mapping;
using StudentPetitions.Api.Models.Petitions;
using StudentPetitions.Api.Repositories.Interfaces;
using StudentPetitions.Api.Services.Implementations;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Tests;

public class PetitionServiceTests
{
    private readonly Mock<IPetitionRepository> petitionRepository = new();
    private readonly Mock<IStudentRepository> studentRepository = new();
    private readonly Mock<ICurrentUserService> currentUserService = new();
    private readonly IMapper mapper = CreateMapper();

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

        studentRepository
            .Setup(repository => repository.GetByIdAsync(request.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Student { Id = request.StudentId });
        petitionRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Petition>(), It.IsAny<CancellationToken>()))
            .Callback<Petition, CancellationToken>((petition, _) => capturedPetition = petition)
            .Returns(Task.CompletedTask);
        petitionRepository
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
        petitionRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Petition>(), It.IsAny<CancellationToken>()),
            Times.Once);
        petitionRepository.Verify(
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

        petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);
        petitionRepository
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
        petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowBusinessRuleException_WhenPetitionIsNotDraft()
    {
        var petition = CreatePetition(PetitionStatus.Submitted);

        petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);

        var service = CreateService();

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.UpdateAsync(petition.Id, CreateUpdatePetitionRequest()));
        petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_ShouldChangeStatusToSubmitted_WhenPetitionIsDraft()
    {
        var petition = CreatePetition(PetitionStatus.Draft);

        petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);
        petitionRepository
            .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.SubmitAsync(petition.Id);

        Assert.Equal(PetitionStatus.Submitted, result.Status);
        Assert.Equal(PetitionStatus.Submitted, petition.Status);
        petitionRepository.Verify(
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

        petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);
        petitionRepository
            .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.ReviewAsync(petition.Id, request);

        Assert.Equal(PetitionStatus.Approved, result.Status);
        Assert.Equal(PetitionStatus.Approved, petition.Status);
        Assert.Equal("Approved after review.", petition.ReviewComment);
        Assert.Equal("Reviewer Admin", petition.ReviewedBy);
        Assert.NotNull(petition.ReviewedAt);
        petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReviewAsync_ShouldThrowBusinessRuleException_WhenPetitionIsNotSubmitted()
    {
        var petition = CreatePetition(PetitionStatus.Draft);

        petitionRepository
            .Setup(repository => repository.GetTrackedByIdAsync(petition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petition);

        var service = CreateService();

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.ReviewAsync(petition.Id, CreateReviewPetitionRequest()));
        petitionRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private PetitionService CreateService()
    {
        return new PetitionService(
            petitionRepository.Object,
            studentRepository.Object,
            currentUserService.Object,
            mapper);
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
