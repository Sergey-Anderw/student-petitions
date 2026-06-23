namespace StudentPetitions.Api.Models.Common;

public sealed record Result<T>(
    ResultStatus Status,
    T? Value = default,
    string? ErrorMessage = null)
{
    public static Result<T> Success(T value)
    {
        return new Result<T>(ResultStatus.Success, value);
    }

    public static Result<T> NotFound(string message)
    {
        return new Result<T>(ResultStatus.NotFound, ErrorMessage: message);
    }

    public static Result<T> Conflict(string message)
    {
        return new Result<T>(ResultStatus.Conflict, ErrorMessage: message);
    }
}
