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

        return result.Status switch
        {
            ResultStatus.Success => CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Id },
                result.Value),
            ResultStatus.NotFound => NotFound(ErrorResponse.NotFound(result.ErrorMessage!)),
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

        return result.Status switch
        {
            ResultStatus.Success => Ok(result.Value),
            ResultStatus.NotFound => NotFound(ErrorResponse.NotFound(result.ErrorMessage!)),
            ResultStatus.Conflict => Conflict(ErrorResponse.Conflict(result.ErrorMessage!)),
            _ => Problem(title: "Unexpected petition update result.", statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
