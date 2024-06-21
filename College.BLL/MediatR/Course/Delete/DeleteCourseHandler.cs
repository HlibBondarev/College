using FluentResults;
using MediatR;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;

using CourseEntity = College.DAL.Entities.Course;

namespace College.BLL.MediatR.Course.Delete;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result<DeleteCourseResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteCourseHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteCourseResponseDto>> Handle(DeleteCourseCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var course = await _repositoryWrapper.CoursesRepository
           .GetFirstOrDefaultAsync(course => course.Id == request.Id);

        if (course is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(CourseEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.CoursesRepository.Delete(course);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeleteCourseError(request);
        }

        var responseDto = new DeleteCourseResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteCourseResponseDto> FailedToDeleteCourseError(DeleteCourseRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(CourseEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}