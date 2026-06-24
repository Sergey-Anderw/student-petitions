# Student Petitions Management API

A REST API for managing student petitions such as course retakes, academic leave, major changes, and other academic requests. Students can be registered, petitions can be created and updated while in draft, and reviewers can process submitted petitions.

## Main Features

- Student registration and lookup
- Paginated student listing
- Petition creation with initial Draft status
- Petition lookup, listing, and filtering
- Draft-only petition updates
- Petition submission and review workflow
- Swagger API documentation
- Application exception handling middleware

## Tech Stack

- ASP.NET Core 8 Web API
- Controllers-based API endpoints
- Entity Framework Core with Code First migrations
- DbContext
- SQLite database
- Fluent API entity configuration
- Repository Pattern
- AutoMapper
- FluentValidation
- Global exception handling middleware returning `ErrorResponse`
- Swagger / OpenAPI

## Architecture Overview

The project follows the assignment-required single-project structure:

- `Controllers/` - API endpoints
- `Services/` - business logic, interfaces, and implementations
- `Repositories/` - database access, interfaces, and implementations
- `Entities/` - EF Core entities
- `Models/` - DTOs, request models, response models, and query/filter models

Supporting folders:

- `Data/` - `AppDbContext`, EF Core configurations, and migrations
- `Infrastructure/` - cross-cutting technical components
- `Infrastructure/Exceptions/` - custom application exceptions
- `Infrastructure/Middleware/` - global exception handling middleware
- `Infrastructure/Mapping/` - AutoMapper profile
- `Extensions/` - dependency injection and middleware registration helpers

Validators are placed near the request DTOs they validate under `Models/`, such as `Models/Students/`.

## API Overview

Students:

- `POST /api/students` - create student
- `GET /api/students/{id}` - get student by id
- `GET /api/students` - get paginated students list

Students API status: implemented.

Petitions:

- `POST /api/petitions` - create petition with Draft status
- `GET /api/petitions/{id}` - get petition by id
- `GET /api/petitions` - get petitions list with filtering
- `PUT /api/petitions/{id}` - update petition only when status is Draft
- `POST /api/petitions/{id}/submit` - submit petition
- `POST /api/petitions/{id}/review` - review petition

Petitions create, get, list, draft update, submit, and review endpoints are implemented.

## Business Rules Summary

- Student email must be unique.
- Student number must be unique.
- New petitions start in Draft status.
- Only Draft petitions can be updated.
- Only Draft petitions can be submitted.
- Review is allowed for Submitted petitions.
- Review requires a review comment.
- The review operation internally moves Submitted petitions to UnderReview before setting Approved or Rejected.
- API date output should use `MM/dd/yyyy` format.

## Running Locally

From the repository root:

```bash
dotnet restore
dotnet build
dotnet run --project StudentPetitions.Api
```

Swagger is available in Development after the API starts.

## Database and Migrations

The API uses EF Core Code First with SQLite. The default local database is configured as `student-petitions.db`.

```bash
dotnet ef migrations add InitialCreate --project StudentPetitions.Api --startup-project StudentPetitions.Api --output-dir Data/Migrations
dotnet ef database update --project StudentPetitions.Api --startup-project StudentPetitions.Api
```

## Authentication

JWT authentication is planned as an optional enhancement if included during implementation.

## Error Handling

Services return response DTOs directly on success. Expected API errors are represented by custom application exceptions:

- `NotFoundException`
- `ConflictException`
- `BusinessRuleException`

`ExceptionHandlingMiddleware` maps these exceptions to `ErrorResponse`. The project does not use `Result<T>`, operation-specific service result records, `ProblemDetails`, or `IExceptionHandler`.

DTO validation is handled by FluentValidation auto-validation. Invalid model state uses the default ASP.NET Core validation response.
