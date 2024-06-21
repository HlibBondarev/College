﻿using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using TeacherEntity = College.DAL.Entities.Teacher;
using Microsoft.EntityFrameworkCore;

namespace College.BLL.MediatR.Teacher.GetById;

public class GetByIdTeacherHandler : IRequestHandler<GetByIdTeacherQuery, Result<GetByIdTeacherResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetByIdTeacherHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<GetByIdTeacherResponseDto>> Handle(GetByIdTeacherQuery request, CancellationToken cancellationToken)
    {
        var teacher = await _repositoryWrapper.TeachersRepository.GetFirstOrDefaultAsync(
            predicate: t => t.Id == request.Id);

        if (teacher is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TeacherEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        var teacherCourses = await _repositoryWrapper.TeachersRepository
                                            .GetAllAsync(
                                            t => t.Id == request.Id,
                                            include: t => t.Include(t => t.Courses));

        var teacherDto = _mapper.Map<GetByIdTeacherResponseDto>(teacher);

        return Result.Ok(teacherDto);
    }
}