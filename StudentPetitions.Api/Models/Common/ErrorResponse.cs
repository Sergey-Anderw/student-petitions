namespace StudentPetitions.Api.Models.Common;

public sealed record ErrorResponse(
    int Status,
    string Title,
    string Detail,
    IDictionary<string, string[]>? Errors = null)
{
    public static ErrorResponse Validation(IDictionary<string, string[]> errors)
    {
        return new ErrorResponse(
            StatusCodes.Status400BadRequest,
            "Validation Error",
            "One or more validation errors occurred.",
            errors);
    }

    public static ErrorResponse NotFound(string detail)
    {
        return new ErrorResponse(StatusCodes.Status404NotFound, "Not Found", detail);
    }

    public static ErrorResponse Conflict(string detail)
    {
        return new ErrorResponse(StatusCodes.Status409Conflict, "Conflict", detail);
    }
}
