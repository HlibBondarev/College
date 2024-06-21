using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using FluentResults;

namespace College.BLL.MediatR.Teacher.Update;

public sealed record UpdateTeacherCommand(UpdateTeacherRequestDto Request) :
    ICommand<Result<UpdateTeacherResponseDto>>;