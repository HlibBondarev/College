using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using CourseEntity = College.DAL.Entities.Course;
using Microsoft.EntityFrameworkCore;

namespace College.BLL.MediatR.Course.GetAll;

public class GetAllCoursesHandler : IRequestHandler<GetAllCoursesQuery, Result<IEnumerable<GetAllCoursesResponseDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetAllCoursesHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<GetAllCoursesResponseDto>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _repositoryWrapper.CoursesRepository.GetAllAsync(
            null,
            include: c => c.Include(c => c.Teacher!));

        if (!courses.Any())
        {
            string errorMsg = string.Format(
            ErrorMessages.EntitiesNotFound,
            typeof(CourseEntity).Name);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        return Result.Ok(_mapper.Map<IEnumerable<GetAllCoursesResponseDto>>(courses));
    }
}