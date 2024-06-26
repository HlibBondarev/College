using FluentResults;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;

namespace College.BLL.MediatR.Course.Delete;

public record DeleteCourseCommand(DeleteCourseRequestDto Request) : ICommand<Result<DeleteCourseResponseDto>>;