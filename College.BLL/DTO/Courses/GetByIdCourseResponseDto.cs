namespace College.BLL.DTO.Courses;

public record GetByIdCourseResponseDto(Guid Id, string? Name, int Duration, string? TeacherName/*, IEnumerable<StudentCoursesDto>? Courses*/);