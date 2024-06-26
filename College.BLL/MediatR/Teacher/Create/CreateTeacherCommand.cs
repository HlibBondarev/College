using FluentResults;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;

namespace College.BLL.MediatR.Teacher.Create;

public record CreateTeacherCommand(CreateTeacherRequestDto Request) : ICommand<Result<CreateTeacherResponseDto>>;
