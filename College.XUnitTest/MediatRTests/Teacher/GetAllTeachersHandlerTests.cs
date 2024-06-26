using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.BLL.MediatR.Teacher.GetAll;
using College.BLL.Resources.Errors;
using College.DAL.Repositories.Interfaces.Base;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.XUnitTest.MediatRTests.Teacher;

public class GetAllTeachersHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetAllTeachersHandlerTests()
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
        var query = new GetAllTeachersQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCollectionOfCorrectCount_WhenEntitiesExist()
    {
        // Arrange
        var mockTexts = GetTeacherList();
        var expectedCount = mockTexts.Count;
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllTeachersQuery();

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
        var query = new GetAllTeachersQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.TeachersRepository.GetAllAsync(null, null), Times.Once);
    }

    [Fact]
    public async Task GetAll_MapperShouldMapOnlyOnce_WhenEntitiesExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllTeachersQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<GetAllTeachersResponseDto>>(It.IsAny<IEnumerable<TeacherEntity>>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCollectionOfTextDto_WhenEntitiesExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllTeachersQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.IsType<List<GetAllTeachersResponseDto>>(result.Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFail_WhenEntitiesAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();
        var handler = CreateHandler();
        var query = new GetAllTeachersQuery();

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
        var query = new GetAllTeachersQuery();
        var expectedError = string.Format(
            ErrorMessages.EntitiesNotFound,
            typeof(TeacherEntity).Name);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal(expectedError, result.Errors[0].Message);
    }

    private GetAllTeachersHandler CreateHandler()
    {
        return new GetAllTeachersHandler(
            repositoryWrapper: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private static List<TeacherEntity> GetTeacherList()
    {
        return new List<TeacherEntity>
            {
                new() {Id = Guid.NewGuid(), Name = "Title1", Degree = "PH Doctor1"},
                new() {Id = Guid.NewGuid(), Name = "Title2", Degree = "PH Doctor2"},
                new() {Id = Guid.NewGuid(), Name = "Title3", Degree = "PH Doctor3"}
            };
    }

    private static List<GetAllTeachersResponseDto> GetTeacherDtoList()
    {
        return new List<GetAllTeachersResponseDto>
            {
                new (Id: Guid.NewGuid(), Name: "Title1", Degree: "PH Doctor1"),
                new (Id: Guid.NewGuid(), Name: "Title2", Degree: "PH Doctor2"),
                new (Id: Guid.NewGuid(), Name: "Title3", Degree: "PH Doctor3")
            };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<GetAllTeachersResponseDto>>(It.IsAny<IEnumerable<TeacherEntity>>()))
            .Returns(GetTeacherDtoList());
    }

    private void MockRepositorySetupReturnsData()
    {
        _mockRepositoryWrapper.Setup(x => x.TeachersRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TeacherEntity>,
            IIncludableQueryable<TeacherEntity, object>>>()))
            .ReturnsAsync(GetTeacherList());
    }

    private void MockRepositorySetupReturnsNull()
    {
        List<TeacherEntity>? list = null;
        _mockRepositoryWrapper.Setup(x => x.TeachersRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TeacherEntity>,
            IIncludableQueryable<TeacherEntity, object>>>()))
            .ReturnsAsync(list);
    }
}