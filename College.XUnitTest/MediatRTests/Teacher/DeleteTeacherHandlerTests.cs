using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.MediatR.Teacher.Delete;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.XUnitTest.MediatRTests.Teacher;

public class DeleteTeacherHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteTeacherHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidDeleteTeacherCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidDeleteTeacherRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeleteTeacherRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new DeleteTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidDeleteTeacherCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidDeleteTeacherRequest();
        var expectedType = typeof(Result<DeleteTeacherResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidDeleteTeacherRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteTeacherCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidDeleteTeacherRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new DeleteTeacherCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.DeleteFailed,
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
        var request = GetValidDeleteTeacherRequest();
        SetupMockWithNotExistingTeacherId(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteTeacherCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TeacherEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private DeleteTeacherHandler CreateHandler()
    {
        return new DeleteTeacherHandler(
            repository: _mockRepositoryWrapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(DeleteTeacherRequestDto request, int saveChangesAsyncResult)
    {
        var teacherEntity = new TeacherEntity
        {
            Id = Guid.NewGuid()
        };

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
            .ReturnsAsync(teacherEntity);

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.Delete(It.IsAny<TeacherEntity>()));

        _mockRepositoryWrapper.
            Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<TeacherEntity>(It.IsAny<DeleteTeacherRequestDto>()))
            .Returns(teacherEntity);
        _mockMapper
            .Setup(m => m.Map<DeleteTeacherResponseDto>(It.IsAny<TeacherEntity>()))
            .Returns(GetValidDeleteTeacherResponse());
    }

    private void SetupMockWithNotExistingTeacherId(DeleteTeacherRequestDto request, int saveChangesAsyncResult)
    {
        TeacherEntity? teacherEntity = null;

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
            .ReturnsAsync(teacherEntity);
    }

    private static DeleteTeacherRequestDto GetValidDeleteTeacherRequest()
    {
        return new DeleteTeacherRequestDto(
            Id: Guid.Empty);
    }

    private static DeleteTeacherResponseDto GetValidDeleteTeacherResponse()
    {
        return new DeleteTeacherResponseDto(IsDeleted: true);
    }
}
