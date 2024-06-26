using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.MediatR.Course.Update;
using CourseEntity = College.DAL.Entities.Course;
using TeacherEntity = College.DAL.Entities.Teacher;
using College.BLL.MediatR.Student.Update;

namespace College.XUnitTest.MediatRTests.Course;

public class UpdateCourseHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateCourseHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidUpdateCourseCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidUpdateCourseRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidUpdateCourseRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new UpdateCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidUpdateCourseCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidUpdateCourseRequest();
        var expectedType = typeof(Result<UpdateCourseResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidUpdateCourseRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateCourseCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidUpdateCourseRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new UpdateCourseCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.UpdateFailed,
            typeof(CourseEntity).Name,
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
        var request = GetValidUpdateCourseRequest();
        SetupMockWithNotExistingTeacherId(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateCourseCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.ForeignKeyByIdNotFound,
            nameof(request.TeacherId),
            request.TeacherId,
            typeof(TeacherEntity).Name);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }


    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentCourseId()
    {
        // Arrange
        var request = GetValidUpdateCourseRequest();
        SetupMockWithNotExistingCourseId(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateCourseCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(CourseEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private UpdateCourseHandler CreateHandler()
    {
        return new UpdateCourseHandler(
            repository: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(UpdateCourseRequestDto request, int saveChangesAsyncResult)
    {
        var courseEntity = new CourseEntity() { Id = Guid.Empty, Name = "Title", Duration = 10 };
        var teacherEntity = new TeacherEntity() { Id = Guid.Empty, Name = "Title", Degree = "PH Doctor" };

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
            .ReturnsAsync(teacherEntity);
        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
            .ReturnsAsync(courseEntity);

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.Create(It.IsAny<CourseEntity>()))
            .Returns(courseEntity);

        _mockRepositoryWrapper
            .Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<CourseEntity>(It.IsAny<UpdateCourseRequestDto>()))
            .Returns(courseEntity);
        _mockMapper
            .Setup(m => m.Map<UpdateCourseResponseDto>(It.IsAny<CourseEntity>()))
            .Returns(GetValidUpdateCourseResponse());
    }

    private void SetupMockWithNotExistingTeacherId(UpdateCourseRequestDto request, int saveChangesAsyncResult)
    {
        TeacherEntity? teacherEntity = null;
        var courseEntity = new CourseEntity() { Id = Guid.Empty, Name = "Title", Duration = 10 };

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
            .ReturnsAsync(courseEntity);

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
            .ReturnsAsync(teacherEntity);

    }

    private void SetupMockWithNotExistingCourseId(UpdateCourseRequestDto request, int saveChangesAsyncResult)
    {
        CourseEntity? courseEntity = null;

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
            .ReturnsAsync(courseEntity);
    }

    private static UpdateCourseRequestDto GetValidUpdateCourseRequest()
    {
        return new UpdateCourseRequestDto(
            Id: Guid.Empty,
            Name: "Title",
            Duration: 10,
            TeacherId: Guid.Empty);
    }

    private static UpdateCourseResponseDto GetValidUpdateCourseResponse()
    {
        return new UpdateCourseResponseDto(Id: Guid.NewGuid(), Name: "title",Duration: 10, Guid.NewGuid());
    }
}