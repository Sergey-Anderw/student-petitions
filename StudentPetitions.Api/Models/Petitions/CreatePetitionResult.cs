namespace StudentPetitions.Api.Models.Petitions;

public sealed record CreatePetitionResult(
    CreatePetitionError Error,
    PetitionResponse? Petition = null);
