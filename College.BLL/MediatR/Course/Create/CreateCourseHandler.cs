using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Courses;
using CourseEntity = College.DAL.Entities.Course;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.BLL.MediatR.Course.Create;

public sealed class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Result<CreateCourseResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateCourseHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<CreateCourseResponseDto>> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (!await IsTeacherExistAsync(request.TeacherId))
        {
            return TeacherNotFoundError(request);
        }

        var courseToCreate = _mapper.Map<CourseEntity>(request);
        var newCourse = _repositoryWrapper.CoursesRepository.Create(courseToCreate);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateCourseError(request);
        }

        var responseDto = _mapper.Map<CreateCourseResponseDto>(newCourse);

        return Result.Ok(responseDto);
    }

    private async Task<bool> IsTeacherExistAsync(Guid teacherId)
    {
        var teacher = await _repositoryWrapper.TeachersRepository
            .GetFirstOrDefaultAsync(s => s.Id == teacherId);

        return teacher is not null;
    }

    private Result<CreateCourseResponseDto> TeacherNotFoundError(CreateCourseRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.ForeignKeyByIdNotFound,
            nameof(request.TeacherId),
            request.TeacherId,
            typeof(TeacherEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<CreateCourseResponseDto> FailedToCreateCourseError(CreateCourseRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(CourseEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}