using StudentPetitions.Api.Extensions;
using StudentPetitions.Api.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiControllers()
    .AddPersistence(builder.Configuration)
    .AddApplicationServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddApiDocumentation();

var app = builder.Build();

app.UseApiDocumentation();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
