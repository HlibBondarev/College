using FluentResults;
using MediatR;
using College.BLL.DTO.Students;

namespace College.BLL.MediatR.Student.GetById;

public record GetByIdStudentQuery(Guid Id) : IRequest<Result<GetByIdStudentResponseDto>>;
