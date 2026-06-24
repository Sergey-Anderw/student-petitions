using StudentPetitions.Api.Infrastructure.Exceptions;
using StudentPetitions.Api.Models.Common;

namespace StudentPetitions.Api.Infrastructure.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
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
            _logger.LogError(exception, "Unhandled exception occurred.");
        }

        context.Response.StatusCode = response.Status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(response);
    }
}
