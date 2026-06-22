using StudentPetitions.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiControllers()
    .AddPersistence(builder.Configuration)
    .AddApplicationServices()
    .AddApiDocumentation();

var app = builder.Build();

app.UseApiDocumentation();

app.UseGlobalExceptionHandling();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
