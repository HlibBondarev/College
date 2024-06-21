namespace College.BLL.DTO.Courses;

public record StudentCoursesDto(Guid StudentId, Guid CourseId, string? Name, int Duration);