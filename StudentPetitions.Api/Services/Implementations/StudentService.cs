using AutoMapper;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Students;
using StudentPetitions.Api.Repositories.Interfaces;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Services.Implementations;

public class StudentService(
    IStudentRepository studentRepository,
    IMapper mapper) : IStudentService
{
    public async Task<Result<StudentResponse>> CreateAsync(
        CreateStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        request.FirstName = request.FirstName.Trim();
        request.LastName = request.LastName.Trim();
        request.Email = request.Email.Trim();
        request.StudentNumber = request.StudentNumber.Trim();

        if (await studentRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return Result<StudentResponse>.Conflict("A student with the same email already exists.");
        }

        if (await studentRepository.ExistsByStudentNumberAsync(request.StudentNumber, cancellationToken))
        {
            return Result<StudentResponse>.Conflict("A student with the same student number already exists.");
        }

        var student = mapper.Map<Student>(request);

        await studentRepository.AddAsync(student, cancellationToken);
        await studentRepository.SaveChangesAsync(cancellationToken);

        return Result<StudentResponse>.Success(mapper.Map<StudentResponse>(student));
    }

    public async Task<StudentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(id, cancellationToken);

        return student is null ? null : mapper.Map<StudentResponse>(student);
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
