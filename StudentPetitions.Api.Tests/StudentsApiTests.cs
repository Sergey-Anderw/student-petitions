using System.Net;
using System.Net.Http.Json;
using StudentPetitions.Api.Models.Common;
using StudentPetitions.Api.Models.Students;

namespace StudentPetitions.Api.Tests;

public class StudentsApiTests(StudentPetitionsApiFactory factory) : IClassFixture<StudentPetitionsApiFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task PostStudents_CreatesStudent()
    {
        var request = CreateStudentRequest();

        var response = await client.PostAsJsonAsync("/api/students", request);
        var student = await response.Content.ReadFromJsonAsync<StudentResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(student);
        Assert.NotEqual(Guid.Empty, student.Id);
        Assert.Equal(request.FirstName, student.FirstName);
        Assert.Equal(request.LastName, student.LastName);
        Assert.Equal(request.Email, student.Email);
        Assert.Equal(request.StudentNumber, student.StudentNumber);
        Assert.False(string.IsNullOrWhiteSpace(student.CreatedAt));
    }

    [Fact]
    public async Task PostStudents_RejectsDuplicateEmail()
    {
        var first = CreateStudentRequest(email: "john@example.com");
        var second = CreateStudentRequest(email: "john@example.com", studentNumber: UniqueStudentNumber());

        await client.PostAsJsonAsync("/api/students", first);

        var response = await client.PostAsJsonAsync("/api/students", second);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task PostStudents_RejectsDuplicateStudentNumber()
    {
        var studentNumber = "ST-001";
        var first = CreateStudentRequest(studentNumber: studentNumber);
        var second = CreateStudentRequest(email: UniqueEmail(), studentNumber: studentNumber);

        await client.PostAsJsonAsync("/api/students", first);

        var response = await client.PostAsJsonAsync("/api/students", second);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetStudentById_ReturnsExistingStudent()
    {
        var created = await CreateStudentAsync();

        var response = await client.GetAsync($"/api/students/{created.Id}");
        var student = await response.Content.ReadFromJsonAsync<StudentResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(student);
        Assert.Equal(created.Id, student.Id);
    }

    [Fact]
    public async Task GetStudentById_ReturnsNotFoundForMissingStudent()
    {
        var response = await client.GetAsync($"/api/students/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetStudents_ReturnsPagedStudents()
    {
        await CreateStudentAsync();
        await CreateStudentAsync();
        await CreateStudentAsync();

        var response = await client.GetAsync("/api/students?pageNumber=1&pageSize=2");
        var page = await response.Content.ReadFromJsonAsync<PagedResponse<StudentResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(page);
        Assert.Equal(2, page.Items.Count);
        Assert.Equal(1, page.PageNumber);
        Assert.Equal(2, page.PageSize);
        Assert.True(page.TotalCount >= 3);
        Assert.True(page.TotalPages >= 2);
    }

    [Fact]
    public async Task PostStudents_ReturnsBadRequestForInvalidRequest()
    {
        var request = new CreateStudentRequest
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = "invalid-email",
            StudentNumber = string.Empty
        };

        var response = await client.PostAsJsonAsync("/api/students", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<StudentResponse> CreateStudentAsync()
    {
        var response = await client.PostAsJsonAsync("/api/students", CreateStudentRequest());
        var student = await response.Content.ReadFromJsonAsync<StudentResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(student);

        return student;
    }

    private static CreateStudentRequest CreateStudentRequest(
        string? email = null,
        string? studentNumber = null)
    {
        return new CreateStudentRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = email ?? UniqueEmail(),
            StudentNumber = studentNumber ?? UniqueStudentNumber()
        };
    }

    private static string UniqueEmail()
    {
        return $"student-{Guid.NewGuid():N}@example.com";
    }

    private static string UniqueStudentNumber()
    {
        return $"ST-{Guid.NewGuid():N}";
    }
}
