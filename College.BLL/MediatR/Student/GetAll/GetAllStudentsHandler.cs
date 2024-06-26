using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Students;
using StudentEntity = College.DAL.Entities.Student;

namespace College.BLL.MediatR.Student.GetAll;

public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, Result<IEnumerable<GetAllStudentsResponseDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetAllStudentsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<GetAllStudentsResponseDto>>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _repositoryWrapper.StudentsRepository.GetAllAsync();
        if (students is null || !students.Any())
        {
            string errorMsg = string.Format(
            ErrorMessages.EntitiesNotFound,
            typeof(StudentEntity).Name);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        return Result.Ok(_mapper.Map<IEnumerable<GetAllStudentsResponseDto>>(students));
    }
}