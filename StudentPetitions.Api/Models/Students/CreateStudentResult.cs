namespace StudentPetitions.Api.Models.Students;

public sealed record CreateStudentResult(
    CreateStudentError Error,
    StudentResponse? Student = null);
