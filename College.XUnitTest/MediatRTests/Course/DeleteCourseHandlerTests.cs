using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using College.BLL.DTO.Students;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Courses;
using College.BLL.MediatR.Course.Delete;
using CourseEntity = College.DAL.Entities.Course;

namespace College.XUnitTest.MediatRTests.Course;

public class DeleteCourseHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteCourseHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidDeleteStudentCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidDeleteCourseRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeleteCourseRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new DeleteCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidDeleteCourseCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidDeleteCourseRequest();
        var expectedType = typeof(Result<DeleteCourseResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidDeleteCourseRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteCourseCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidDeleteCourseRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new DeleteCourseCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(CourseEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentCourseId()
    {
        // Arrange
        var request = GetValidDeleteCourseRequest();
        SetupMockWithNotExistingCourseId(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new DeleteCourseCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(CourseEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private DeleteCourseHandler CreateHandler()
    {
        return new DeleteCourseHandler(
            repository: _mockRepositoryWrapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(DeleteCourseRequestDto request, int saveChangesAsyncResult)
    {
        var courseEntity = new CourseEntity
        {
            Id = Guid.NewGuid()
        };

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
            .ReturnsAsync(courseEntity);

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.Delete(It.IsAny<CourseEntity>()));

        _mockRepositoryWrapper.
            Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<CourseEntity>(It.IsAny<DeleteStudentRequestDto>()))
            .Returns(courseEntity);
        _mockMapper
            .Setup(m => m.Map<DeleteCourseResponseDto>(It.IsAny<CourseEntity>()))
            .Returns(GetValidDeleteCourseResponse());
    }

    private void SetupMockWithNotExistingCourseId(DeleteCourseRequestDto request, int saveChangesAsyncResult)
    {
        CourseEntity? studentEntity = null;

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
            .ReturnsAsync(studentEntity);
    }

    private static DeleteCourseRequestDto GetValidDeleteCourseRequest()
    {
        return new DeleteCourseRequestDto(
            Id: Guid.Empty);
    }

    private static DeleteCourseResponseDto GetValidDeleteCourseResponse()
    {
        return new DeleteCourseResponseDto(IsDeleted: true);
    }
}