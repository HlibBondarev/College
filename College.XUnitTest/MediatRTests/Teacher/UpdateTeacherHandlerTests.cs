using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.MediatR.Teacher.Update;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.XUnitTest.MediatRTests.Teacher;

public class UpdateTeacherHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateTeacherHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidUpdateTeacherCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidUpdateTeacherRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidUpdateTeacherRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new UpdateTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidUpdateTeacherCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidUpdateTeacherRequest();
        var expectedType = typeof(Result<UpdateTeacherResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidUpdateTeacherRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateTeacherCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidUpdateTeacherRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new UpdateTeacherCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.UpdateFailed,
            typeof(TeacherEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentTeacherId()
    {
        // Arrange
        var request = GetValidUpdateTeacherRequest();
        SetupMockWithNotExistingTeacherId(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateTeacherCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TeacherEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private UpdateTeacherHandler CreateHandler()
    {
        return new UpdateTeacherHandler(
            repository: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(UpdateTeacherRequestDto request, int saveChangesAsyncResult)
    {
        var teacherEntity = new TeacherEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Degree = request.Degree,
        };

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
            .ReturnsAsync(teacherEntity);

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.Create(It.IsAny<TeacherEntity>()))
            .Returns(teacherEntity);

        _mockRepositoryWrapper.
            Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<TeacherEntity>(It.IsAny<UpdateTeacherRequestDto>()))
            .Returns(teacherEntity);
        _mockMapper
            .Setup(m => m.Map<UpdateTeacherResponseDto>(It.IsAny<TeacherEntity>()))
            .Returns(GetValidUpdateTeacherResponse());
    }

    private void SetupMockWithNotExistingTeacherId(UpdateTeacherRequestDto request, int saveChangesAsyncResult)
    {
        TeacherEntity? teacherEntity = null;

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
            .ReturnsAsync(teacherEntity);
    }

    private static UpdateTeacherRequestDto GetValidUpdateTeacherRequest()
    {
        return new UpdateTeacherRequestDto(
            Id: Guid.NewGuid(),
            Name: "Title",
            Degree: "PH Doctor");
    }

    private static UpdateTeacherResponseDto GetValidUpdateTeacherResponse()
    {
        return new UpdateTeacherResponseDto(Id: Guid.NewGuid(), Name: "title", Degree: "PH Doctor");
    }
}
