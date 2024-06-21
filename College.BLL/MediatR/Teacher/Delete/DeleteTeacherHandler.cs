using FluentResults;
using MediatR;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using TeacherEntity = College.DAL.Entities.Teacher;


namespace College.BLL.MediatR.Teacher.Delete;

public class DeleteTeacherHandler : IRequestHandler<DeleteTeacherCommand, Result<DeleteTeacherResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteTeacherHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteTeacherResponseDto>> Handle(DeleteTeacherCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var teacher = await _repositoryWrapper.TeachersRepository
           .GetFirstOrDefaultAsync(teacher => teacher.Id == request.Id);

        if (teacher is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TeacherEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.TeachersRepository.Delete(teacher);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeletePartnerError(request);
        }

        var responseDto = new DeleteTeacherResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteTeacherResponseDto> FailedToDeletePartnerError(DeleteTeacherRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(TeacherEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}

