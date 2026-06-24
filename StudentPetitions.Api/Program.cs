using StudentPetitions.Api.Extensions;
using StudentPetitions.Api.Infrastructure.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddApiControllers()
    .AddPersistence(builder.Configuration)
    .AddApplicationServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddApiDocumentation();

var app = builder.Build();

app.UseApiDocumentation();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.SeedDemoDataAsync();

app.Run();

public partial class Program;
