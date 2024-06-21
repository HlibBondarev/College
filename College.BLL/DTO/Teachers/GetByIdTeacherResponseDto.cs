using College.DAL.Entities;

namespace College.BLL.DTO.Teachers;

public record GetByIdTeacherResponseDto(Guid Id, string? Name, string? Degree, IEnumerable<Course>? Courses);