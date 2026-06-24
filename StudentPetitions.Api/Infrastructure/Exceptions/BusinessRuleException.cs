namespace StudentPetitions.Api.Infrastructure.Exceptions;

public sealed class BusinessRuleException(string message) : Exception(message);

public sealed class ConflictException(string message) : Exception(message);

public sealed class NotFoundException(string message) : Exception(message);

public sealed class UnauthorizedException(string message) : Exception(message);
