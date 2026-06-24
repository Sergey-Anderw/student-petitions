using StudentPetitions.Api.Extensions;
using StudentPetitions.Api.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiControllers()
    .AddPersistence(builder.Configuration)
    .AddApplicationServices()
    .AddApiDocumentation();

var app = builder.Build();

app.UseApiDocumentation();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
