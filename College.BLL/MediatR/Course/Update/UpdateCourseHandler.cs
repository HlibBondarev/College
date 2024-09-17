using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Entities;
using College.DAL.Repositories.Interfaces.Base;
using CourseEntity = College.DAL.Entities.Course;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.BLL.MediatR.Course.Update;

public sealed class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Result<UpdateCourseResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public UpdateCourseHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UpdateCourseResponseDto>> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        using var transaction = _repositoryWrapper.BeginTransaction();

        var courseToUpdate = _mapper.Map<CourseEntity>(request);
        var existedCourse = await _repositoryWrapper.CoursesRepository
            .GetFirstOrDefaultAsync(course => course.Id == request.Id);

        if (existedCourse is null)
        {
            return CourseNotFoundError(request);
        }

        if (!await IsTeacherExistAsync(request.TeacherId))
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

        _repositoryWrapper.CoursesRepository.Update(courseToUpdate);
        courseToUpdate.Students.Clear();
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToUpdateCourseError(request);
        }

        var oldStudents = await _repositoryWrapper.StudentCourseRepository
           .GetAllAsync(sc => sc.CourseId == courseToUpdate.Id);

        foreach (var old in oldStudents!)
        {
            if (!request.CourseStudents.Contains(old.StudentId))
            {
                _repositoryWrapper.StudentCourseRepository.Delete(old);
            }
        }

        foreach (var newStudentId in request.CourseStudents!)
        {
            if (oldStudents.FirstOrDefault(sc => sc.StudentId == newStudentId) == null)
            {
                await _repositoryWrapper.StudentCourseRepository.CreateAsync(
                    new StudentCourse() { StudentId = newStudentId, CourseId = courseToUpdate.Id });
            }
        }

        await _repositoryWrapper.SaveChangesAsync();

        transaction.Complete();

        var responseDto = _mapper.Map<UpdateCourseResponseDto>(courseToUpdate);
        responseDto.CourseStudents = request.CourseStudents;

        return Result.Ok(responseDto);
    }

    private Result<UpdateCourseResponseDto> CourseNotFoundError(UpdateCourseRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(CourseEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }

    private async Task<bool> IsTeacherExistAsync(Guid teacherId)
    {
        var teacher = await _repositoryWrapper.TeachersRepository
            .GetFirstOrDefaultAsync(s => s.Id == teacherId);

        return teacher is not null;
    }

    private Result<UpdateCourseResponseDto> TeacherNotFoundError(UpdateCourseRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.ForeignKeyByIdNotFound,
            nameof(request.TeacherId),
            request.TeacherId,
            typeof(TeacherEntity).Name);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }

    private static bool AreStudentsTheSameAsync(UpdateCourseRequestDto request)
    {
        return request.CourseStudents.Distinct().Count() != request.CourseStudents.Count;
    }

    private Result<UpdateCourseResponseDto> StudentsAreTheSameError(UpdateCourseRequestDto request)
    {
        string errorMsg = "Two or more students passed in the request are the same.";
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> AreStudentsExistedtAsync(UpdateCourseRequestDto request)
    {
        var students = await _repositoryWrapper.StudentsRepository.GetAllAsync(s => request.CourseStudents.Contains(s.Id));
        return students.Count() == request.CourseStudents.Count;
    }

    private Result<UpdateCourseResponseDto> StudentsNotExistedError(UpdateCourseRequestDto request)
    {
        string errorMsg = "The students passed in the request do not exist.";
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<UpdateCourseResponseDto> FailedToUpdateCourseError(UpdateCourseRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.UpdateFailed,
            typeof(CourseEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }
}

