using StudentPetitions.Api.Infrastructure.Exceptions;
using StudentPetitions.Api.Models.Common;

namespace StudentPetitions.Api.Infrastructure.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = exception switch
        {
            NotFoundException => ErrorResponse.NotFound(exception.Message),
            ConflictException => ErrorResponse.Conflict(exception.Message),
            BusinessRuleException => ErrorResponse.Conflict(exception.Message),
            UnauthorizedException => ErrorResponse.Unauthorized(exception.Message),
            _ => ErrorResponse.InternalServerError("An unexpected error occurred.")
        };

        if (response.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception occurred.");
        }

        context.Response.StatusCode = response.Status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(response);
    }
}
