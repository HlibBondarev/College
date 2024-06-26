using AutoMapper;
using FluentResults;
using MediatR;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.BLL.MediatR.Teacher.GetAll;

public class GetAllTeachersHandler : IRequestHandler<GetAllTeachersQuery, Result<IEnumerable<GetAllTeachersResponseDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetAllTeachersHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<GetAllTeachersResponseDto>>> Handle(GetAllTeachersQuery request, CancellationToken cancellationToken)
    {
        var teachers = await _repositoryWrapper.TeachersRepository.GetAllAsync();

        if (teachers is null || !teachers.Any())
        {
            string errorMsg = string.Format(
            ErrorMessages.EntitiesNotFound,
            typeof(TeacherEntity).Name);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        return Result.Ok(_mapper.Map<IEnumerable<GetAllTeachersResponseDto>>(teachers));
    }
}
