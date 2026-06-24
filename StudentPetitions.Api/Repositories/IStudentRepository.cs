using Microsoft.EntityFrameworkCore;
using StudentPetitions.Api.Data;
using StudentPetitions.Api.Entities;

namespace StudentPetitions.Api.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Student>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<(bool EmailExists, bool StudentNumberExists)> GetUniquenessConflictAsync(
        string email,
        string studentNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(Student student, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class StudentRepository(AppDbContext dbContext) : IStudentRepository
{
    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Student>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Students
            .AsNoTracking()
            .OrderBy(student => student.CreatedAt)
            .ThenBy(student => student.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Students.CountAsync(cancellationToken);
    }

    public async Task<(bool EmailExists, bool StudentNumberExists)> GetUniquenessConflictAsync(
        string email,
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();
        var normalizedStudentNumber = studentNumber.Trim();

        var matches = await dbContext.Students
            .AsNoTracking()
            .Where(student =>
                student.Email.ToLower() == normalizedEmail ||
                student.StudentNumber == normalizedStudentNumber)
            .Select(student => new
            {
                EmailMatches = student.Email.ToLower() == normalizedEmail,
                StudentNumberMatches = student.StudentNumber == normalizedStudentNumber
            })
            .ToListAsync(cancellationToken);

        return (
            matches.Any(match => match.EmailMatches),
            matches.Any(match => match.StudentNumberMatches));
    }

    public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        await dbContext.Students.AddAsync(student, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
