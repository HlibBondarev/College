using FluentResults;
using MediatR;
using College.BLL.DTO.Teachers;

namespace College.BLL.MediatR.Teacher.GetById;

public record GetByIdTeacherQuery(Guid Id) : IRequest<Result<GetByIdTeacherResponseDto>>;
