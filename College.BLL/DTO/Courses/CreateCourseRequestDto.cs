namespace College.BLL.DTO.Courses;

public sealed record CreateCourseRequestDto(string? Name, int Duration, Guid TeacherId, List<Guid> CourseStudents);