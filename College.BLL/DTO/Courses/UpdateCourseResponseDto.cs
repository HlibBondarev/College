namespace College.BLL.DTO.Courses;

public sealed record UpdateCourseResponseDto(Guid Id, string? Name, int? Duration, Guid TeacherId);