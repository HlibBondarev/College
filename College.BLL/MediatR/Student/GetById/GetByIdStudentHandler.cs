using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Students;
using StudentEntity = College.DAL.Entities.Student;

namespace College.BLL.MediatR.Student.GetById;
public class GetByIdStudentHandler : IRequestHandler<GetByIdStudentQuery, Result<GetByIdStudentResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetByIdStudentHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<GetByIdStudentResponseDto>> Handle(GetByIdStudentQuery request, CancellationToken cancellationToken)
    {
        var student = await _repositoryWrapper.StudentsRepository.GetFirstOrDefaultAsync(
            predicate: t => t.Id == request.Id,
            include: t => t.Include(t => t.Courses));

        if (student is null)
        {
            string errorMsg = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StudentEntity).Name,
            request.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        var studentDto = _mapper.Map<GetByIdStudentResponseDto>(student);

        return Result.Ok(studentDto);
    }
}