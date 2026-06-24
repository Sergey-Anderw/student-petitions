using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPetitions.Api.Models.Petitions;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Controllers;

[ApiController]
[Route("api/petitions")]
public class PetitionsController(IPetitionService petitionService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<PetitionResponse>> Create(
        [FromBody]
        CreatePetitionRequest request,
        CancellationToken cancellationToken)
    {
        var petition = await petitionService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = petition.Id },
            petition);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Student,Reviewer")]
    public async Task<ActionResult<PetitionResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var petition = await petitionService.GetByIdAsync(id, cancellationToken);

        return Ok(petition);
    }

    [HttpGet]
    [Authorize(Roles = "Student,Reviewer")]
    public async Task<ActionResult<IReadOnlyCollection<PetitionResponse>>> GetFiltered(
        [FromQuery] PetitionFilterRequest filter,
        CancellationToken cancellationToken)
    {
        var petitions = await petitionService.GetFilteredAsync(filter, cancellationToken);

        return Ok(petitions);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<PetitionResponse>> Update(
        Guid id,
        [FromBody]
        UpdatePetitionRequest request,
        CancellationToken cancellationToken)
    {
        var petition = await petitionService.UpdateAsync(id, request, cancellationToken);

        return Ok(petition);
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<PetitionResponse>> Submit(Guid id, CancellationToken cancellationToken)
    {
        var petition = await petitionService.SubmitAsync(id, cancellationToken);

        return Ok(petition);
    }

    [HttpPost("{id:guid}/review")]
    [Authorize(Roles = "Reviewer")]
    public async Task<ActionResult<PetitionResponse>> Review(
        Guid id,
        [FromBody]
        ReviewPetitionRequest request,
        CancellationToken cancellationToken)
    {
        var petition = await petitionService.ReviewAsync(id, request, cancellationToken);

        return Ok(petition);
    }
}
