using FluentResults;
using MediatR;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Students;
using StudentEntity = College.DAL.Entities.Student;

namespace College.BLL.MediatR.Student.Delete;

public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, Result<DeleteStudentResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteStudentHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repositoryWrapper = repository;
        _logger = logger;
    }

    public async Task<Result<DeleteStudentResponseDto>> Handle(DeleteStudentCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var student = await _repositoryWrapper.StudentsRepository
           .GetFirstOrDefaultAsync(student => student.Id == request.Id);

        if (student is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StudentEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        _repositoryWrapper.StudentsRepository.Delete(student);
        bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            return FailedToDeleteStudentError(request);
        }

        var responseDto = new DeleteStudentResponseDto(true);

        return Result.Ok(responseDto);
    }

    private Result<DeleteStudentResponseDto> FailedToDeleteStudentError(DeleteStudentRequestDto request)
    {
        string errorMsg = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(StudentEntity).Name,
            request.Id);
        _logger.LogError(request, errorMsg);
        return Result.Fail(errorMsg);
    }
}
