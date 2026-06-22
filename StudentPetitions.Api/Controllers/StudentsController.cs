using Microsoft.AspNetCore.Mvc;
using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Students;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<StudentResponse>> Create(
        [FromBody]
        CreateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await studentService.CreateAsync(request, cancellationToken);

        return result.Error switch
        {
            CreateStudentError.None => CreatedAtAction(
                nameof(GetById),
                new { id = result.Student!.Id },
                result.Student),
            CreateStudentError.EmailAlreadyExists => Conflict(
                ErrorResponse.Conflict("A student with the same email already exists.")),
            CreateStudentError.StudentNumberAlreadyExists => Conflict(
                ErrorResponse.Conflict("A student with the same student number already exists.")),
            _ => Problem(title: "Unexpected student creation result.", statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var student = await studentService.GetByIdAsync(id, cancellationToken);

        if (student is null)
        {
            return NotFound(ErrorResponse.NotFound("Student was not found."));
        }

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
