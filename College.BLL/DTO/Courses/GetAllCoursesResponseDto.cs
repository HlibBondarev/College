namespace College.BLL.DTO.Courses;

public sealed record GetAllCoursesResponseDto(Guid Id, string? Name, int Duration, string? TeacherName);