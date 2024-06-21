using College.BLL.DTO.Courses;
using FluentResults;
using MediatR;

namespace College.BLL.MediatR.Course.GetAll;

public record GetAllCoursesQuery() : IRequest<Result<IEnumerable<GetAllCoursesResponseDto>>>;