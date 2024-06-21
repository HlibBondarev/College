namespace College.BLL.DTO.Teachers;

public sealed record UpdateTeacherRequestDto(Guid Id, string? Name, string? Degree);