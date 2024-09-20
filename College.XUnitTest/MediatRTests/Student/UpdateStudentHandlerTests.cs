using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using College.BLL.DTO.Students;
using College.BLL.Interfaces;
using College.BLL.MediatR.Student.Update;
using College.BLL.Resources.Errors;
using College.DAL.Entities;
using College.DAL.Repositories.Interfaces.Base;
using StudentEntity = College.DAL.Entities.Student;
using CourseEntity = College.DAL.Entities.Course;


namespace College.XUnitTest.MediatRTests.Student;

public class UpdateStudentHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;
    const int SUCCESSFULSAVETWICE = 2;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateStudentHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidUpdateStudentCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidUpdateStudentRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidUpdateStudentRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new UpdateStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidUpdateStudentCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidUpdateStudentRequest();
        var expectedType = typeof(Result<UpdateStudentResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncTwice_IfInputIsValid()
    {
        // Arrange
        var request = GetValidUpdateStudentRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateStudentCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(SUCCESSFULSAVETWICE));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidUpdateStudentRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new UpdateStudentCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.UpdateFailed,
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
        var request = GetValidUpdateStudentRequest();
        SetupMockWithNotExistingStudentId(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateStudentCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StudentEntity).Name,
            request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private UpdateStudentHandler CreateHandler()
    {
        return new UpdateStudentHandler(
            repository: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(UpdateStudentRequestDto request, int saveChangesAsyncResult)
    {
        var courseList = new List<CourseEntity>() 
        { 
            new CourseEntity() 
            { 
                Id = Guid.Empty, 
                Name = "Title", 
                Duration = 10 
            } 
        };
        var studentEntity = new StudentEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            DateOfBirth = DateTime.Now,
            Courses = new List<CourseEntity>()
        };

        _mockRepositoryWrapper
            .Setup(r => r.StudentsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
            .ReturnsAsync(studentEntity);

        _mockRepositoryWrapper
            .Setup(x => x.BeginTransaction())
            .Returns(new System.Transactions.TransactionScope());

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.GetAllAsync(It.IsAny<Expression<Func<CourseEntity, bool>>>(), null))
            .ReturnsAsync(courseList);

        _mockRepositoryWrapper
            .Setup(r => r.StudentCourseRepository.GetAllAsync(It.IsAny<Expression<Func<StudentCourse, bool>>>(), null))
            .ReturnsAsync(new List<StudentCourse>());

        _mockRepositoryWrapper
            .Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<StudentEntity>(It.IsAny<UpdateStudentRequestDto>()))
            .Returns(studentEntity);

        _mockMapper
            .Setup(m => m.Map<UpdateStudentResponseDto>(It.IsAny<StudentEntity>()))
            .Returns(GetValidUpdateStudentResponse());
    }

    private void SetupMockWithNotExistingStudentId(UpdateStudentRequestDto request, int saveChangesAsyncResult)
    {
        StudentEntity? courseEntity = null;

        _mockRepositoryWrapper
            .Setup(r => r.StudentsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
            .ReturnsAsync(courseEntity);
    }

    private static UpdateStudentRequestDto GetValidUpdateStudentRequest()
    {
        return new UpdateStudentRequestDto(
            Id: Guid.NewGuid(),
            Name: "Title",
            DateOfBirth: new DateTime(year: DateTime.Now.Year - 25, month: 1, day: 1),
            StudentCourses: new List<Guid>() { Guid.Empty });
    }

    private static UpdateStudentResponseDto GetValidUpdateStudentResponse()
    {
        return new UpdateStudentResponseDto()
        {
            Id = Guid.NewGuid(),
            Name = "title",
            DateOfBirth = new DateTime(year: DateTime.Now.Year - 25, month: 1, day: 1)
        };
    }
}

