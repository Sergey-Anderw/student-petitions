namespace StudentPetitions.Api.Infrastructure.Exceptions;

public sealed class UnauthorizedException(string message) : Exception(message);
