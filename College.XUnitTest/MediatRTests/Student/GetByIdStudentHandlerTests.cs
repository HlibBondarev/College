using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.DTO.Students;
using College.BLL.MediatR.Student.GetById;
using StudentEntity = College.DAL.Entities.Student;

namespace College.XUnitTest.MediatRTests.Student;

public class GetByIdStudentHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetByIdStudentHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_IfIdExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsEntity(id);
        MockMapperSetup(id);
        var handler = CreateHandler();
        var query = new GetByIdStudentQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetById_RepositoryShouldCallGetFirstOrDefaultAsyncOnlyTwice_IfEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsEntity(id);
        MockMapperSetup(id);
        var handler = CreateHandler();
        var query = new GetByIdStudentQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(
            r =>
            r.StudentsRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<StudentEntity, bool>>>(),
               It.IsAny<Func<IQueryable<StudentEntity>,
               IIncludableQueryable<StudentEntity, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetById_MapperShouldCallMapOnlyOnce_IfEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsEntity(id);
        MockMapperSetup(id);
        var handler = CreateHandler();
        var query = new GetByIdStudentQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockMapper.Verify(
            m => m.Map<GetByIdStudentResponseDto>(It.IsAny<StudentEntity>()),
            Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnTextWithCorrectId_IfEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsEntity(id);
        MockMapperSetup(id);
        var handler = CreateHandler();
        var query = new GetByIdStudentQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal(id, result.Value.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnTextDto_IfEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsEntity(id);
        MockMapperSetup(id);
        var handler = CreateHandler();
        var query = new GetByIdStudentQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.IsType<GetByIdStudentResponseDto>(result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnFail_WhenEntityIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsNull();
        var handler = CreateHandler();
        var query = new GetByIdStudentQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetById_ShouldLogCorrectErrorMessage_WhenEntityIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsNull();
        var handler = CreateHandler();
        var query = new GetByIdStudentQuery(id);

        var expected = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StudentEntity).Name,
            id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);
        var actual = result.Errors[0].Message;

        // Assert
        Assert.Equal(expected, actual);
    }

    private GetByIdStudentHandler CreateHandler()
    {
        return new GetByIdStudentHandler(
            repositoryWrapper: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void MockMapperSetup(Guid id)
    {
        _mockMapper.Setup(x => x
            .Map<GetByIdStudentResponseDto>(It.IsAny<StudentEntity>()))
            .Returns(new GetByIdStudentResponseDto
            (
                Id: id,
                Name: "Name",
                DateOfBirth: new DateTime(year: DateTime.Now.Year - 25, month: 1, day: 1),
                Courses: new List<CourseDto>()
            ));
    }

    private void MockRepositorySetupReturnsEntity(Guid id)
    {
        _mockRepositoryWrapper.Setup(x => x.StudentsRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<StudentEntity, bool>>>(),
               It.IsAny<Func<IQueryable<StudentEntity>,
               IIncludableQueryable<StudentEntity, object>>>()))
            .ReturnsAsync(new StudentEntity { Id = id });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.StudentsRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<StudentEntity, bool>>>(),
               It.IsAny<Func<IQueryable<StudentEntity>,
               IIncludableQueryable<StudentEntity, object>>>()))
            .ReturnsAsync((StudentEntity?)null);
    }
}
