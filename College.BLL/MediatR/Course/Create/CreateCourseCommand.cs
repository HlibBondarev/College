using FluentResults;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;

namespace College.BLL.MediatR.Course.Create;

public record CreateCourseCommand(CreateCourseRequestDto Request) : ICommand<Result<CreateCourseResponseDto>>;
