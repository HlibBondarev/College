using College.BLL.DTO.Students;
using College.BLL.Interfaces;
using FluentResults;

namespace College.BLL.MediatR.Student.Create;

public record CreateStudentCommand(CreateStudentRequestDto Request) : ICommand<Result<CreateStudentResponseDto>>;
