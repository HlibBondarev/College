using FluentResults;
using MediatR;
using College.BLL.DTO.Students;

namespace College.BLL.MediatR.Student.GetAll;

public record GetAllStudentsQuery() : IRequest<Result<IEnumerable<GetAllStudentsResponseDto>>>;
