using StudentPetitions.Api.Data;
using StudentPetitions.Api.Data.Seed;

namespace StudentPetitions.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }

    public static async Task SeedDemoDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await DemoDataSeeder.SeedAsync(dbContext);
    }
}
