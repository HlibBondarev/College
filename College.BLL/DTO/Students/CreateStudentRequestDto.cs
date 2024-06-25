namespace College.BLL.DTO.Students;

public sealed record CreateStudentRequestDto(string? Name, DateTime DateOfBirth, List<Guid> StudentCourses);