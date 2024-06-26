using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using CourseEntity = College.DAL.Entities.Course;

namespace College.BLL.MediatR.Course.GetById;

public class GetByIdCourseHandler : IRequestHandler<GetByIdCourseQuery, Result<GetByIdCourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetByIdCourseHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<GetByIdCourseResponseDto>> Handle(GetByIdCourseQuery request, CancellationToken cancellationToken)
    {
        var course = await _repositoryWrapper.CoursesRepository.GetFirstOrDefaultAsync(
            predicate: t => t.Id == request.Id,
            include: c => c.Include(c => c.Teacher!));

        if (course is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(CourseEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        var courseWithStudents = await _repositoryWrapper.CoursesRepository.GetFirstOrDefaultAsync(
            predicate: t => t.Id == request.Id,
            include: c => c.Include(c => c.Students));

        course.Students = courseWithStudents!.Students;

        var courseDto = _mapper.Map<GetByIdCourseResponseDto>(course);

        return Result.Ok(courseDto);
    }
}