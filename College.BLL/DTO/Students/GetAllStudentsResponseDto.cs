namespace College.BLL.DTO.Students;

public sealed record GetAllStudentsResponseDto(Guid Id, string? Name, DateTime DateOfBirth);