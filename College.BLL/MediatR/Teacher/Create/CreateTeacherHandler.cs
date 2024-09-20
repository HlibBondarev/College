using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.Resources.Errors;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.BLL.MediatR.Teacher.Create;

public sealed class CreateTeacherHandler : IRequestHandler<CreateTeacherCommand, Result<CreateTeacherResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateTeacherHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<CreateTeacherResponseDto>> Handle(CreateTeacherCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var teacherToCreate = _mapper.Map<TeacherEntity>(request);

        var newTeacher = _repositoryWrapper.TeachersRepository.Create(teacherToCreate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateTeacherError(request);
        }

        var responseDto = _mapper.Map<CreateTeacherResponseDto>(newTeacher);

        return Result.Ok(responseDto);
    }

    private Result<CreateTeacherResponseDto> FailedToCreateTeacherError(CreateTeacherRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.CreateFailed,
            typeof(TeacherEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}