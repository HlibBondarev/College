using FluentResults;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;

namespace College.BLL.MediatR.Course.Update;

public sealed record UpdateCourseCommand(UpdateCourseRequestDto Request) :
    ICommand<Result<UpdateCourseResponseDto>>;