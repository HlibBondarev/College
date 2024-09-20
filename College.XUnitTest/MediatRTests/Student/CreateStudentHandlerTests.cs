using AutoMapper;
using FluentResults;
using FluentAssertions;
using Moq;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Students;
using College.BLL.MediatR.Student.Create;
using System.Linq.Expressions;
using StudentEntity = College.DAL.Entities.Student;
using CoursetEntity = College.DAL.Entities.Course;

namespace College.XUnitTest.MediatRTests.Student;

public class CreateStudentHandlerTests
{
    const int FAILEDSAVE = -1;
    const int SUCCESSFULSAVE = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateStudentHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ValidCreateStudentCommand_ShouldSucceed()
    {
        // Arrange
        var request = GetValidCreateStudentRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateStudentRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new CreateStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCreateStudentCommand_ShouldReturnResultOfCorrectType()
    {
        // Arrange
        var request = GetValidCreateStudentRequest();
        var expectedType = typeof(Result<CreateStudentResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStudentCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateStudentRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStudentCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandIsInvalid()
    {
        // Arrange
        var request = GetValidCreateStudentRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = CreateHandler();
        var command = new CreateStudentCommand(request);
        var expectedErrorMessage = string.Format(
            ErrorMessages.CreateFailed,
            typeof(StudentEntity).Name);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private CreateStudentHandler CreateHandler()
    {
        return new CreateStudentHandler(
            repository: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void SetupMock(CreateStudentRequestDto request, int saveChangesAsyncResult)
    {
        var courseList = new List<CoursetEntity>() { new CoursetEntity() { Id = Guid.Empty, Name = "Title", Duration = 10 } };
        var studentEntity = new StudentEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            DateOfBirth = DateTime.Now,
            Courses = courseList
        };

        _mockRepositoryWrapper
            .Setup(x => x.BeginTransaction())
            .Returns(new System.Transactions.TransactionScope());

        _mockRepositoryWrapper
            .Setup(r => r.CoursesRepository.GetAllAsync(It.IsAny<Expression<Func<CoursetEntity, bool>>>(), null))
            .ReturnsAsync(courseList);

        _mockRepositoryWrapper
            .Setup(r => r.StudentsRepository.Create(It.IsAny<StudentEntity>()))
            .Returns(studentEntity);

        _mockRepositoryWrapper
            .Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<StudentEntity>(It.IsAny<CreateStudentRequestDto>()))
            .Returns(studentEntity);
        _mockMapper
            .Setup(m => m.Map<CreateStudentResponseDto>(It.IsAny<StudentEntity>()))
            .Returns(GetValidCreateStudentResponse());
    }

    private static CreateStudentRequestDto GetValidCreateStudentRequest()
    {
        return new CreateStudentRequestDto(
            Name: "Title",
            DateOfBirth: new DateTime(year: DateTime.Now.Year - 25, month: 1, day: 1),
            StudentCourses: new List<Guid>() { Guid.Empty });
    }

    private static CreateStudentResponseDto GetValidCreateStudentResponse()
    {
        return new CreateStudentResponseDto(Id: Guid.NewGuid());
    }
}
