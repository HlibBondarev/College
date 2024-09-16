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

        using var transaction = _repositoryWrapper.BeginTransaction();

        if (!await IsTeacherExistedtAsync(request.TeacherId))
        {
            return TeacherNotFoundError(request);
        }

        if (AreStudentsTheSameAsync(request))
        {
            return StudentsAreTheSameError(request);
        }

        if (!await AreStudentsExistedtAsync(request))
        {
            return StudentsNotExistedError(request);
        }

        var courseToCreate = _mapper.Map<CourseEntity>(request);
        courseToCreate = _repositoryWrapper.CoursesRepository.Create(courseToCreate);
        courseToCreate.Students.Clear();
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateCourseError(request);
        }

        var students = await _repositoryWrapper.StudentsRepository.GetAllAsync(c => request.CourseStudents.Contains(c.Id));

        if (students.Any())
        {
            courseToCreate.Students.AddRange(students);
            resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                return FailedToCreateCourseError(request);
            }
        }

        transaction.Complete();

        return Result.Ok(_mapper.Map<CreateCourseResponseDto>(courseToCreate));
    }

    private async Task<bool> IsTeacherExistedtAsync(Guid teacherId)
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

    private bool AreStudentsTheSameAsync(CreateCourseRequestDto request)
    {
        return request.CourseStudents.Distinct().Count() != request.CourseStudents.Count;
    }

    private Result<CreateCourseResponseDto> StudentsAreTheSameError(CreateCourseRequestDto request)
    {
        string errorMsg = string.Format("Two or more students passed in the request are the same.");
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> AreStudentsExistedtAsync(CreateCourseRequestDto request)
    {
        var students = await _repositoryWrapper.StudentsRepository.GetAllAsync(s => request.CourseStudents.Contains(s.Id));
        return students.Count() == request.CourseStudents.Count;
    }

    private Result<CreateCourseResponseDto> StudentsNotExistedError(CreateCourseRequestDto request)
    {
        string errorMsg = string.Format("The students passed in the request do not exist.");
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