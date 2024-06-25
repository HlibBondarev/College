using FluentResults;
using College.BLL.DTO.Students;
using College.BLL.Interfaces;

namespace College.BLL.MediatR.Student.Update;

public sealed record UpdateStudentCommand(UpdateStudentRequestDto Request) :
    ICommand<Result<UpdateStudentResponseDto>>;