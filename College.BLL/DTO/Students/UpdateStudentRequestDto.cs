namespace College.BLL.DTO.Students;

public sealed record UpdateStudentRequestDto(Guid Id, string? Name, DateTime DateOfBirth, List<Guid> StudentCourses);