using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.Resources.Errors;
using TeacherEntity = College.DAL.Entities.Teacher;


namespace College.BLL.MediatR.Teacher.Update;

public sealed class UpdateTeacherHandler : IRequestHandler<UpdateTeacherCommand, Result<UpdateTeacherResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public UpdateTeacherHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UpdateTeacherResponseDto>> Handle(UpdateTeacherCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var textToUpdate = _mapper.Map<TeacherEntity>(request);

        var existedTeacher = await _repositoryWrapper.TeachersRepository
            .GetFirstOrDefaultAsync(teacher => teacher.Id == request.Id);

        if (existedTeacher is null)
        {
            return TeacherNotFoundError(request);
        }

        //textToUpdate.Id = existedTeacher.Id;

        _repositoryWrapper.TeachersRepository.Update(textToUpdate);

        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToCreateTeacherError(request);
        }

        var responseDto = _mapper.Map<UpdateTeacherResponseDto>(textToUpdate);

        return Result.Ok(responseDto);
    }

    private Result<UpdateTeacherResponseDto> TeacherNotFoundError(UpdateTeacherRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TeacherEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }

    private Result<UpdateTeacherResponseDto> FailedToCreateTeacherError(UpdateTeacherRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.UpdateFailed,
            typeof(TeacherEntity).Name);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}
