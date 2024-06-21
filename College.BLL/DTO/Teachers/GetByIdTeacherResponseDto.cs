using College.BLL.DTO.Courses;

namespace College.BLL.DTO.Teachers;

public record GetByIdTeacherResponseDto(Guid Id, string? Name, string? Degree, IEnumerable<TeacherCoursesDto>? Courses);