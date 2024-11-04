using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Courses;
using College.BLL.MediatR.Course.Create;
using CourseEntity = College.DAL.Entities.Course;
using TeacherEntity = College.DAL.Entities.Teacher;
using StudentEntity = College.DAL.Entities.Student;

namespace College.XUnitTest.MediatRTests.Course;

public class CreateCourseHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateCourseHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidCreateCourseCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidCreateCourseRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateCourseRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new CreateCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCreateCourseCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidCreateCourseRequest();
        var expectedType = typeof(Result<CreateCourseResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateCourseCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncTwice_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateCourseRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateCourseCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidCreateCourseRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new CreateCourseCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.CreateFailed,
            typeof(CourseEntity).Name);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentTeacherId()
    {
        // Arrange
        var request = GetValidCreateCourseRequest();
        SetupMockWithNotExistingTeacherId(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateCourseCommand(request);
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

    private CreateCourseHandler CreateHandler()
    {
        return new CreateCourseHandler(
            repository: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(CreateCourseRequestDto request, int saveChangesAsyncResult)
    {
        var courseEntity = new CourseEntity() 
        { 
            Id = Guid.Empty,
            Name = "Title",
            Duration = 10,
            Students = new List<StudentEntity>()
        };
        var studentList = new List<StudentEntity>()
        {
            new StudentEntity()
            {
                Id = Guid.Empty,
                Name = "Student"
            }
        };
        var teacherEntity = new TeacherEntity() 
        { 
            Id = Guid.Empty,
            Name = "Teacher", 
            Degree = "PH Doctor" 
        };

        _mockRepositoryWrapper
            .Setup(x => x.BeginTransaction())
            .Returns(new System.Transactions.TransactionScope());

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
            .ReturnsAsync(teacherEntity);

        _mockRepositoryWrapper
            .Setup(r => r.StudentsRepository.GetAllAsync(It.IsAny<Expression<Func<StudentEntity, bool>>>(), null))
            .ReturnsAsync(studentList);

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.CreateAsync(It.IsAny<CourseEntity>()))
            .ReturnsAsync(courseEntity);

        _mockRepositoryWrapper
            .Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<CourseEntity>(It.IsAny<CreateCourseRequestDto>()))
            .Returns(courseEntity);
        _mockMapper
            .Setup(m => m.Map<CreateCourseResponseDto>(It.IsAny<CourseEntity>()))
            .Returns(GetValidCreateCourseResponse());
    }

    private void SetupMockWithNotExistingTeacherId(CreateCourseRequestDto request, int saveChangesAsyncResult)
    {
        TeacherEntity? teacherEntity = null;

        _mockRepositoryWrapper
            .Setup(r => r.TeachersRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TeacherEntity, bool>>>(), null))
            .ReturnsAsync(teacherEntity);
    }

    private static CreateCourseRequestDto GetValidCreateCourseRequest()
    {
        return new CreateCourseRequestDto(
            Name: "Title",
            Duration: 10,
            TeacherId: Guid.Empty,
            CourseStudents: new List<Guid>() { Guid.Empty });
    }

    private static CreateCourseResponseDto GetValidCreateCourseResponse()
    {
        return new CreateCourseResponseDto(Id: Guid.NewGuid());
    }
}

