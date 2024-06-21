using College.BLL.DTO.Teachers;
using FluentResults;
using MediatR;

namespace College.BLL.MediatR.Teacher.GetAll;

public record GetAllTeachersQuery() : IRequest<Result<IEnumerable<GetAllTeachersResponseDto>>>;
