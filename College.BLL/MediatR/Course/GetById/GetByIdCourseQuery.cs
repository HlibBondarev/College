using FluentResults;
using MediatR;
using College.BLL.DTO.Courses;

namespace College.BLL.MediatR.Course.GetById;

public record GetByIdCourseQuery(Guid Id) : IRequest<Result<GetByIdCourseResponseDto>>;
