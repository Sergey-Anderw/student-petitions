using System.Text.Json.Serialization;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentPetitions.Api.Data;
using StudentPetitions.Api.Infrastructure.Mapping;
using StudentPetitions.Api.Models.Students;
using StudentPetitions.Api.Repositories.Implementations;
using StudentPetitions.Api.Repositories.Interfaces;
using StudentPetitions.Api.Services.Implementations;
using StudentPetitions.Api.Services.Interfaces;

namespace StudentPetitions.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateStudentRequestValidator>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Missing required configuration value: Jwt:Key");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IPetitionRepository, PetitionRepository>();
        services.AddScoped<IPetitionService, PetitionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter a JWT token."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
