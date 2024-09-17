namespace College.BLL.DTO.Courses;

public sealed record UpdateCourseRequestDto(Guid Id, string? Name, int? Duration, Guid TeacherId, List<Guid> CourseStudents);