using College.BLL.DTO.Courses;

namespace College.BLL.DTO.Students;

public record GetByIdStudentResponseDto(Guid Id, string? Name, DateTime DateOfBirth, IEnumerable<CourseDto> Courses);