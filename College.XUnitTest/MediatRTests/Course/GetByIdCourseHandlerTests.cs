using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using College.BLL.DTO.Courses;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.MediatR.Course.GetById;
using CourseEntity = College.DAL.Entities.Course;

namespace College.XUnitTest.MediatRTests.Course;

public class GetByIdCourseHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetByIdCourseHandlerTests()
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
        var query = new GetByIdCourseQuery(id);

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
        var query = new GetByIdCourseQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(
            r =>
            r.CoursesRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<CourseEntity, bool>>>(),
               It.IsAny<Func<IQueryable<CourseEntity>,
               IIncludableQueryable<CourseEntity, object>>>()),
            Times.AtMost(2));
    }

    [Fact]
    public async Task GetById_MapperShouldCallMapOnlyOnce_IfEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsEntity(id);
        MockMapperSetup(id);
        var handler = CreateHandler();
        var query = new GetByIdCourseQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockMapper.Verify(
            m => m.Map<GetByIdCourseResponseDto>(It.IsAny<CourseEntity>()),
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
        var query = new GetByIdCourseQuery(id);

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
        var query = new GetByIdCourseQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.IsType<GetByIdCourseResponseDto>(result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnFail_WhenEntityIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsNull();
        var handler = CreateHandler();
        var query = new GetByIdCourseQuery(id);

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
        var query = new GetByIdCourseQuery(id);

        var expected = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(CourseEntity).Name,
            id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);
        var actual = result.Errors[0].Message;

        // Assert
        Assert.Equal(expected, actual);
    }

    private GetByIdCourseHandler CreateHandler()
    {
        return new GetByIdCourseHandler(
            repositoryWrapper: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void MockMapperSetup(Guid id)
    {
        _mockMapper.Setup(x => x
            .Map<GetByIdCourseResponseDto>(It.IsAny<CourseEntity>()))
            .Returns(new GetByIdCourseResponseDto()
            {
                Id = id,
                Name = "Name",
                Duration=10,
                TeacherName= "Name"
            });
    }

    private void MockRepositorySetupReturnsEntity(Guid id)
    {
        _mockRepositoryWrapper.Setup(x => x.CoursesRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<CourseEntity, bool>>>(),
               It.IsAny<Func<IQueryable<CourseEntity>,
               IIncludableQueryable<CourseEntity, object>>>()))
            .ReturnsAsync(new CourseEntity { Id = id });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.CoursesRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<CourseEntity, bool>>>(),
               It.IsAny<Func<IQueryable<CourseEntity>,
               IIncludableQueryable<CourseEntity, object>>>()))
            .ReturnsAsync((CourseEntity?)null);
    }
}
