using Microsoft.AspNetCore.Mvc;
using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Petitions;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Controllers;

[ApiController]
[Route("api/petitions")]
public class PetitionsController(IPetitionService petitionService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PetitionResponse>> Create(
        [FromBody]
        CreatePetitionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await petitionService.CreateAsync(request, cancellationToken);

        return result.Error switch
        {
            CreatePetitionError.None => CreatedAtAction(
                nameof(GetById),
                new { id = result.Petition!.Id },
                result.Petition),
            CreatePetitionError.StudentNotFound => NotFound(ErrorResponse.NotFound("Student was not found.")),
            _ => Problem(title: "Unexpected petition creation result.", statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PetitionResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var petition = await petitionService.GetByIdAsync(id, cancellationToken);

        if (petition is null)
        {
            return NotFound(ErrorResponse.NotFound("Petition was not found."));
        }

        return Ok(petition);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<PetitionResponse>>> GetPaged(
        [FromQuery] PetitionFilterRequest filter,
        CancellationToken cancellationToken)
    {
        var petitions = await petitionService.GetPagedAsync(filter, cancellationToken);

        return Ok(petitions);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PetitionResponse>> Update(
        Guid id,
        [FromBody]
        UpdatePetitionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await petitionService.UpdateAsync(id, request, cancellationToken);

        return result.Error switch
        {
            UpdatePetitionError.None => Ok(result.Petition),
            UpdatePetitionError.PetitionNotFound => NotFound(ErrorResponse.NotFound("Petition was not found.")),
            UpdatePetitionError.PetitionIsNotDraft => Conflict(
                ErrorResponse.Conflict("Only draft petitions can be updated.")),
            _ => Problem(title: "Unexpected petition update result.", statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
