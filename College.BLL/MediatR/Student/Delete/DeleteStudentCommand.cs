using FluentResults;
using College.BLL.Interfaces;
using College.BLL.DTO.Students;

namespace College.BLL.MediatR.Student.Delete;

public record DeleteStudentCommand(DeleteStudentRequestDto Request) : ICommand<Result<DeleteStudentResponseDto>>;