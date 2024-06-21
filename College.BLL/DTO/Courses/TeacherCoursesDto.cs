namespace College.BLL.DTO.Courses;

public record TeacherCoursesDto(Guid TeacherId, Guid CourseId, string? Name, int Duration);