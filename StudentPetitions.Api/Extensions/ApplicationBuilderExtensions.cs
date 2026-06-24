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
}
