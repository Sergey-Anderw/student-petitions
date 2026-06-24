using Microsoft.EntityFrameworkCore;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Infrastructure.Auth;

namespace StudentPetitions.Api.Data.Seed;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var demoStudentExists = await dbContext.Students
            .AnyAsync(student => student.Id == DemoUsers.StudentId, cancellationToken);

        if (demoStudentExists)
        {
            return;
        }

        dbContext.Students.Add(new Student
        {
            Id = DemoUsers.StudentId,
            FirstName = "Demo",
            LastName = "Student",
            Email = "student@example.com",
            StudentNumber = "DEMO-STUDENT-001",
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
