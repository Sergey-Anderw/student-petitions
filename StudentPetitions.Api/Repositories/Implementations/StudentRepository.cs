using Microsoft.EntityFrameworkCore;
using StudentPetitions.Api.Data;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Repositories.Interfaces;

namespace StudentPetitions.Api.Repositories.Implementations;

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
            .OrderByDescending(student => student.CreatedAt)
            .ThenBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Students.CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Email.ToLower() == normalizedEmail, cancellationToken);
    }

    public async Task<bool> ExistsByStudentNumberAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedStudentNumber = studentNumber.Trim();

        return await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.StudentNumber == normalizedStudentNumber, cancellationToken);
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
