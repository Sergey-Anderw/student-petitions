namespace StudentPetitions.Api.Models.Common;

public sealed record ErrorResponse(
    int Status,
    string Title,
    string Detail)
{
    public static ErrorResponse NotFound(string detail)
    {
        return new ErrorResponse(StatusCodes.Status404NotFound, "Not Found", detail);
    }

    public static ErrorResponse Conflict(string detail)
    {
        return new ErrorResponse(StatusCodes.Status409Conflict, "Conflict", detail);
    }

    public static ErrorResponse Unauthorized(string detail)
    {
        return new ErrorResponse(StatusCodes.Status401Unauthorized, "Unauthorized", detail);
    }

    public static ErrorResponse InternalServerError(string detail)
    {
        return new ErrorResponse(StatusCodes.Status500InternalServerError, "Server Error", detail);
    }
}
