using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.DTO.Students;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using StudentEntity = College.DAL.Entities.Student;

namespace College.BLL.MediatR.Student.Create;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Result<CreateStudentResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateStudentHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateStudentResponseDto>> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        using var transaction = _repositoryWrapper.BeginTransaction();

        if (AreCoursesTheSameAsync(request))
        {
            return CoursesAreTheSameError(request);
        }

        if (!await AreCoursesExistedtAsync(request))
        {
            return CoursesNotExistedError(request);
        }

        var studentToCreate = _mapper.Map<StudentEntity>(request);
        studentToCreate = _repositoryWrapper.StudentsRepository.Create(studentToCreate);

        studentToCreate.Courses.Clear();

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (!resultIsSuccess)
        {
            return FailedToCreateStudentError(request);
        }

        var courses = await _repositoryWrapper.CoursesRepository.GetAllAsync(c => request.StudentCourses.Contains(c.Id));

        if (courses.Any())
        {
            studentToCreate.Courses.AddRange(courses);
            resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                return FailedToCreateStudentError(request);
            }
        }

        transaction.Complete();

        return Result.Ok(_mapper.Map<CreateStudentResponseDto>(studentToCreate));
    }

    private bool AreCoursesTheSameAsync(CreateStudentRequestDto request)
    {
        return request.StudentCourses.Distinct().Count() != request.StudentCourses.Count;
    }

    private Result<CreateStudentResponseDto> CoursesAreTheSameError(CreateStudentRequestDto request)
    {
        string errorMsg = string.Format("Two or more courses passed in the request are the same.");
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private async Task<bool> AreCoursesExistedtAsync(CreateStudentRequestDto request)
    {
        var courses = await _repositoryWrapper.CoursesRepository.GetAllAsync(c => request.StudentCourses.Contains(c.Id));
        return courses.Count() == request.StudentCourses.Count;
    }

    private Result<CreateStudentResponseDto> CoursesNotExistedError(CreateStudentRequestDto request)
    {
        string errorMsg = string.Format("The courses passed in the request do not exist.");
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<CreateStudentResponseDto> FailedToCreateStudentError(CreateStudentRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(StudentEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}