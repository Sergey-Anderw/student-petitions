using AutoMapper;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Infrastructure.Exceptions;
using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Students;
using StudentPetitions.Api.Repositories;

namespace StudentPetitions.Api.Services;

public interface IStudentService
{
    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);

    Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResponse<StudentResponse>> GetPagedAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);
}

public class StudentService(
    IStudentRepository studentRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IStudentService
{
    public async Task<StudentResponse> CreateAsync(
        CreateStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        request.FirstName = request.FirstName.Trim();
        request.LastName = request.LastName.Trim();
        request.Email = request.Email.Trim();
        request.StudentNumber = request.StudentNumber.Trim();

        var conflict = await studentRepository.GetUniquenessConflictAsync(
            request.Email,
            request.StudentNumber,
            cancellationToken);

        if (conflict.EmailExists)
        {
            throw new ConflictException("A student with the same email already exists.");
        }

        if (conflict.StudentNumberExists)
        {
            throw new ConflictException("A student with the same student number already exists.");
        }

        var student = mapper.Map<Student>(request);

        await studentRepository.AddAsync(student, cancellationToken);
        await studentRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<StudentResponse>(student);
    }

    public async Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (currentUserService.IsStudent && currentUserService.StudentId != id)
        {
            throw new NotFoundException("Student was not found.");
        }

        var student = await studentRepository.GetByIdAsync(id, cancellationToken);

        if (student is null)
        {
            throw new NotFoundException("Student was not found.");
        }

        return mapper.Map<StudentResponse>(student);
    }

    public async Task<PagedResponse<StudentResponse>> GetPagedAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var students = await studentRepository.GetPagedAsync(
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var totalCount = await studentRepository.CountAsync(cancellationToken);
        var responses = mapper.Map<IReadOnlyCollection<StudentResponse>>(students);

        return new PagedResponse<StudentResponse>(
            responses,
            query.PageNumber,
            query.PageSize,
            totalCount);
    }
}
