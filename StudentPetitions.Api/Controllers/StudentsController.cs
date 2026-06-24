using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Students;
using StudentPetitions.Api.Services;

namespace StudentPetitions.Api.Controllers;

[ApiController]
[Route("api/students")]
[Authorize(Roles = "Student,Reviewer")]
public sealed class StudentsController(IStudentService studentService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<StudentResponse>> Create(
        [FromBody]
        CreateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var student = await studentService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = student.Id },
            student);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var student = await studentService.GetByIdAsync(id, cancellationToken);

        return Ok(student);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<StudentResponse>>> GetPaged(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var students = await studentService.GetPagedAsync(query, cancellationToken);

        return Ok(students);
    }
}
