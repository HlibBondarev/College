using College.BLL.DTO.Teachers;
using FluentResults;
using MediatR;

namespace College.BLL.MediatR.Teacher.GetById;

public record GetByIdTeacherQuery(Guid Id) : IRequest<Result<GetByIdTeacherResponseDto>>;
