using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Infrastructure.Exceptions;
using StudentPetitions.Api.Infrastructure.Mapping;
using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Students;
using StudentPetitions.Api.Repositories.Interfaces;
using StudentPetitions.Api.Services.Implementations;

namespace StudentPetitions.Api.Tests;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> studentRepository = new();
    private readonly IMapper mapper = CreateMapper();

    [Fact]
    public async Task CreateAsync_ShouldCreateStudentAndTrimInput_WhenRequestIsValid()
    {
        Student? capturedStudent = null;
        var request = new CreateStudentRequest
        {
            FirstName = " John ",
            LastName = " Smith ",
            Email = " john@example.com ",
            StudentNumber = " ST-001 "
        };

        studentRepository
            .Setup(repository => repository.GetUniquenessConflictAsync(
                "john@example.com",
                "ST-001",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, false));

        studentRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
            .Callback<Student, CancellationToken>((student, _) => capturedStudent = student)
            .Returns(Task.CompletedTask);

        studentRepository
            .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal("ST-001", result.StudentNumber);
        Assert.NotNull(capturedStudent);
        Assert.Equal("John", capturedStudent.FirstName);
        Assert.Equal("Smith", capturedStudent.LastName);
        Assert.Equal("john@example.com", capturedStudent.Email);
        Assert.Equal("ST-001", capturedStudent.StudentNumber);
        studentRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()),
            Times.Once);
        studentRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
    {
        var request = CreateStudentRequest();

        studentRepository
            .Setup(repository => repository.GetUniquenessConflictAsync(
                request.Email,
                request.StudentNumber,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, false));

        var service = CreateService();

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));

        studentRepository.Verify(
            repository => repository.GetUniquenessConflictAsync(
                request.Email,
                request.StudentNumber,
                It.IsAny<CancellationToken>()),
            Times.Once);
        studentRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()),
            Times.Never);
        studentRepository.Verify(
            repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResponse()
    {
        var query = new PaginationQuery
        {
            PageNumber = 1,
            PageSize = 2
        };
        var students = new List<Student>
        {
            CreateStudent(),
            CreateStudent()
        };

        studentRepository
            .Setup(repository => repository.GetPagedAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);
        studentRepository
            .Setup(repository => repository.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var service = CreateService();

        var result = await service.GetPagedAsync(query);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    private StudentService CreateService()
    {
        return new StudentService(studentRepository.Object, mapper);
    }

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            configurationExpression => configurationExpression.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance);

        return configuration.CreateMapper();
    }

    private static CreateStudentRequest CreateStudentRequest()
    {
        return new CreateStudentRequest
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john@example.com",
            StudentNumber = "ST-001"
        };
    }

    private static Student CreateStudent()
    {
        return new Student
        {
            FirstName = "John",
            LastName = "Smith",
            Email = $"student-{Guid.NewGuid():N}@example.com",
            StudentNumber = $"ST-{Guid.NewGuid():N}"
        };
    }
}
