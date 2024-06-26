using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Students;
using College.BLL.MediatR.Student.Delete;
using StudentEntity = College.DAL.Entities.Student;


namespace College.XUnitTest.MediatRTests.Student;

public class DeleteStudentHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteStudentHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidDeleteStudentCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidDeleteStudentRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeleteStudentRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new DeleteStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidDeleteStudentCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidDeleteStudentRequest();
        var expectedType = typeof(Result<DeleteStudentResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidDeleteStudentRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteStudentCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidDeleteStudentRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new DeleteStudentCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(StudentEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentStudentId()
    {
        // Arrange
        var request = GetValidDeleteStudentRequest();
        SetupMockWithNotExistingStudentId(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteStudentCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StudentEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private DeleteStudentHandler CreateHandler()
    {
        return new DeleteStudentHandler(
            repository: _mockRepositoryWrapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(DeleteStudentRequestDto request, int saveChangesAsyncResult)
    {
        var studentEntity = new StudentEntity
        {
            Id = Guid.NewGuid()
        };

        _mockRepositoryWrapper
            .Setup(r => r.StudentsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
            .ReturnsAsync(studentEntity);

        _mockRepositoryWrapper
            .Setup(r => r.StudentsRepository.Delete(It.IsAny<StudentEntity>()));

        _mockRepositoryWrapper.
            Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<StudentEntity>(It.IsAny<DeleteStudentRequestDto>()))
            .Returns(studentEntity);
        _mockMapper
            .Setup(m => m.Map<DeleteStudentResponseDto>(It.IsAny<StudentEntity>()))
            .Returns(GetValidDeleteStudentResponse());
    }

    private void SetupMockWithNotExistingStudentId(DeleteStudentRequestDto request, int saveChangesAsyncResult)
    {
        StudentEntity? studentEntity = null;

        _mockRepositoryWrapper
            .Setup(r => r.StudentsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
            .ReturnsAsync(studentEntity);
    }

    private static DeleteStudentRequestDto GetValidDeleteStudentRequest()
    {
        return new DeleteStudentRequestDto(
            Id: Guid.Empty);
    }

    private static DeleteStudentResponseDto GetValidDeleteStudentResponse()
    {
        return new DeleteStudentResponseDto(IsDeleted: true);
    }
}
