using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Students;

namespace StudentPetitions.Api.Services.Interfaces;

public interface IStudentService
{
    Task<Result<StudentResponse>> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);

    Task<StudentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResponse<StudentResponse>> GetPagedAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);
}
