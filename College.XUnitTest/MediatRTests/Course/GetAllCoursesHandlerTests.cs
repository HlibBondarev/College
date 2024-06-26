using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.BLL.DTO.Courses;
using College.BLL.MediatR.Course.GetAll;
using College.DAL.Repositories.Interfaces.Base;
using CourseEntity = College.DAL.Entities.Course;

namespace College.XUnitTest.MediatRTests.Course;

public class GetAllCoursesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetAllCoursesHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenEntitiesExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllCoursesQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCollectionOfCorrectCount_WhenEntitiesExist()
    {
        // Arrange
        var mockTexts = GetCourseList();
        var expectedCount = mockTexts.Count;
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllCoursesQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);
        var actualCount = result.Value.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async Task GetAll_RepositoryShouldCallGetAllAsyncOnlyOnce_WhenEntitiesExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllCoursesQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.CoursesRepository.GetAllAsync(
                It.IsAny<Expression<Func<CourseEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CourseEntity>, IIncludableQueryable<CourseEntity, object>>>()), 
            Times.Once);
    }

    [Fact]
    public async Task GetAll_MapperShouldMapOnlyOnce_WhenEntitiesExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllCoursesQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<GetAllCoursesResponseDto>>(It.IsAny<IEnumerable<CourseEntity>>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCollectionOfTextDto_WhenEntitiesExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllCoursesQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.IsType<List<GetAllCoursesResponseDto>>(result.Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFail_WhenEntitiesAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();
        var handler = CreateHandler();
        var query = new GetAllCoursesQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetAll_ShouldLogCorrectErrorMessage_WhenEntitiesAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();
        var handler = CreateHandler();
        var query = new GetAllCoursesQuery();
        var expectedError = string.Format(
            ErrorMessages.EntitiesNotFound,
            typeof(CourseEntity).Name);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal(expectedError, result.Errors[0].Message);
    }

    private GetAllCoursesHandler CreateHandler()
    {
        return new GetAllCoursesHandler(
            repositoryWrapper: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private static List<CourseEntity> GetCourseList()
    {
        return new List<CourseEntity>
            {
                new() {Id = Guid.NewGuid(), Name = "Title1",Duration = 10},
                new() {Id = Guid.NewGuid(), Name = "Title2",Duration = 10},
                new() {Id = Guid.NewGuid(), Name = "Title3",Duration = 10}
            };
    }

    private static List<GetAllCoursesResponseDto> GetCourseDtoList()
    {
        return new List<GetAllCoursesResponseDto>
            {
                new (Id: Guid.NewGuid(), Name: "Title1", Duration: 10, TeacherName: "John"),
                new (Id: Guid.NewGuid(), Name: "Title2", Duration: 10, TeacherName: "Paul"),
                new (Id: Guid.NewGuid(), Name: "Title3", Duration: 10, TeacherName: "Pete")
            };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<GetAllCoursesResponseDto>>(It.IsAny<IEnumerable<CourseEntity>>()))
            .Returns(GetCourseDtoList());
    }

    private void MockRepositorySetupReturnsData()
    {
        _mockRepositoryWrapper.Setup(x => x.CoursesRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<CourseEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CourseEntity>, IIncludableQueryable<CourseEntity, object>>>()))
            .ReturnsAsync(GetCourseList());
    }

    private void MockRepositorySetupReturnsNull()
    {
        List<CourseEntity>? list = null;
        _mockRepositoryWrapper.Setup(x => x.CoursesRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<CourseEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CourseEntity>,
            IIncludableQueryable<CourseEntity, object>>>()))
            .ReturnsAsync(list);
    }
}
