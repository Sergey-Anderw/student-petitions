namespace StudentPetitions.Api.Models.Petitions;

public sealed record UpdatePetitionResult(
    UpdatePetitionError Error,
    PetitionResponse? Petition = null);
