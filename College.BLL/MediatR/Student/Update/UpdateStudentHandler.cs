using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Students;
using College.DAL.Entities;
using StudentEntity = College.DAL.Entities.Student;

namespace College.BLL.MediatR.Student.Update;

public sealed class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, Result<UpdateStudentResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public UpdateStudentHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UpdateStudentResponseDto>> Handle(UpdateStudentCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        using var transaction = _repositoryWrapper.BeginTransaction();

        var studentToUpdate = _mapper.Map<StudentEntity>(request);
        var existedStudent = await _repositoryWrapper.StudentsRepository
            .GetFirstOrDefaultAsync(student => student.Id == request.Id);

        if (existedStudent is null)
        {
            return StudentNotFoundError(request);
        }

        if (AreCoursesTheSameAsync(request))
        {
            return CoursesAreTheSameError(request);
        }

        if (!await AreCoursesExisedtAsync(request))
        {
            return CoursesNotExistedError(request);
        }

        _repositoryWrapper.StudentsRepository.Update(studentToUpdate);
        studentToUpdate.Courses.Clear();
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToUpdateStudentError(request);
        }

        var oldCourses = await _repositoryWrapper.StudentCourseRepository
           .GetAllAsync(sc => sc.StudentId == studentToUpdate.Id);

        foreach (var old in oldCourses!)
        {
            if (!request.StudentCourses.Contains(old.CourseId))
            {
                _repositoryWrapper.StudentCourseRepository.Delete(old);
            }
        }

        foreach (var newCourseId in request.StudentCourses!)
        {
            if (oldCourses.FirstOrDefault(sc => sc.CourseId == newCourseId) == null)
            {
                await _repositoryWrapper.StudentCourseRepository.CreateAsync(
                    new StudentCourse() { StudentId = studentToUpdate.Id, CourseId = newCourseId });
            }
        }

        await _repositoryWrapper.SaveChangesAsync();

        transaction.Complete();

        var responseDto = _mapper.Map<UpdateStudentResponseDto>(studentToUpdate);
        responseDto.StudentCourses = request.StudentCourses;

        return Result.Ok(responseDto);
    }

    private Result<UpdateStudentResponseDto> StudentNotFoundError(UpdateStudentRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StudentEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }

    private static bool AreCoursesTheSameAsync(UpdateStudentRequestDto request)
    {
        return request.StudentCourses.Distinct().Count() != request.StudentCourses.Count;
    }

    private Result<UpdateStudentResponseDto> CoursesAreTheSameError(UpdateStudentRequestDto request)
    {
        string errorMsg = "Two or more courses passed in the request are the same.";
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> AreCoursesExisedtAsync(UpdateStudentRequestDto request)
    {
        var courses = await _repositoryWrapper.CoursesRepository.GetAllAsync(c => request.StudentCourses.Contains(c.Id));
        return courses.Count() == request.StudentCourses.Count;
    }

    private Result<UpdateStudentResponseDto> CoursesNotExistedError(UpdateStudentRequestDto request)
    {
        string errorMsg = "One or more courses passed in the request do not exist.";
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<UpdateStudentResponseDto> FailedToUpdateStudentError(UpdateStudentRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.UpdateFailed,
            typeof(StudentEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }
}
