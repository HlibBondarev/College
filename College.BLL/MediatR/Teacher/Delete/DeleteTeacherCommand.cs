using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using FluentResults;

namespace College.BLL.MediatR.Teacher.Delete;

public record DeleteTeacherCommand(DeleteTeacherRequestDto Request) : ICommand<Result<DeleteTeacherResponseDto>>;