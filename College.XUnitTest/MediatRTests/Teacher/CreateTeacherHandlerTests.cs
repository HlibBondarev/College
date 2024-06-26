using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using College.BLL.Interfaces;
using College.BLL.MediatR.Teacher.Create;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Teachers;
using College.BLL.Resources.Errors;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.XUnitTest.MediatRTests.Teacher;

public class CreateTeacherHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateTeacherHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidCreateTeacherCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidCreateTeacherRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateTeacherRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new CreateTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCreateTeacherCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidCreateTeacherRequest();
        var expectedType = typeof(Result<CreateTeacherResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateTeacherCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateTeacherRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateTeacherCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidCreateTeacherRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new CreateTeacherCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.CreateFailed,
            typeof(TeacherEntity).Name);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private CreateTeacherHandler CreateHandler()
    {
        return new CreateTeacherHandler(
            repository: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(CreateTeacherRequestDto request, int saveChangesAsyncResult)
    {
        var teacherEntity = new TeacherEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Degree = request.Degree,
        };

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.Create(It.IsAny<TeacherEntity>()))
            .Returns(teacherEntity);

        _mockRepositoryWrapper.
            Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<TeacherEntity>(It.IsAny<CreateTeacherRequestDto>()))
            .Returns(teacherEntity);
        _mockMapper
            .Setup(m => m.Map<CreateTeacherResponseDto>(It.IsAny<TeacherEntity>()))
            .Returns(GetValidCreateTeacherResponse());
    }

    private static CreateTeacherRequestDto GetValidCreateTeacherRequest()
    {
        return new CreateTeacherRequestDto(
            Name: "Title",
            Degree: "PH Doctor");
    }

    private static CreateTeacherResponseDto GetValidCreateTeacherResponse()
    {
        return new CreateTeacherResponseDto(Id: Guid.NewGuid());
    }
}
