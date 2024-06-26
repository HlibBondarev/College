using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using College.BLL.Interfaces;
using College.BLL.Resources.Errors;
using College.BLL.MediatR.Student.GetAll;
using College.BLL.DTO.Students;
using College.DAL.Repositories.Interfaces.Base;
using StudentEntity = College.DAL.Entities.Student;

namespace College.XUnitTest.MediatRTests.Student;

public class GetAllStudentsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetAllStudentsHandlerTests()
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
        var query = new GetAllStudentsQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCollectionOfCorrectCount_WhenEntitiesExist()
    {
        // Arrange
        var mockTexts = GetStudentList();
        var expectedCount = mockTexts.Count;
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllStudentsQuery();

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
        var query = new GetAllStudentsQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.StudentsRepository.GetAllAsync(null, null), Times.Once);
    }

    [Fact]
    public async Task GetAll_MapperShouldMapOnlyOnce_WhenEntitiesExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllStudentsQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<GetAllStudentsResponseDto>>(It.IsAny<IEnumerable<StudentEntity>>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCollectionOfTextDto_WhenEntitiesExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handler = CreateHandler();
        var query = new GetAllStudentsQuery();

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.IsType<List<GetAllStudentsResponseDto>>(result.Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFail_WhenEntitiesAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();
        var handler = CreateHandler();
        var query = new GetAllStudentsQuery();

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
        var query = new GetAllStudentsQuery();
        var expectedError = string.Format(
            ErrorMessages.EntitiesNotFound,
            typeof(StudentEntity).Name);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal(expectedError, result.Errors[0].Message);
    }

    private GetAllStudentsHandler CreateHandler()
    {
        return new GetAllStudentsHandler(
            repositoryWrapper: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private static List<StudentEntity> GetStudentList()
    {
        return new List<StudentEntity>
            {
                new() {Id = Guid.NewGuid(), Name = "Title1", DateOfBirth = new DateTime(year: DateTime.Now.Year - 25, month: 1, day: 1)},
                new() {Id = Guid.NewGuid(), Name = "Title2", DateOfBirth = new DateTime(year: DateTime.Now.Year - 26, month: 1, day: 1)},
                new() {Id = Guid.NewGuid(), Name = "Title3", DateOfBirth = new DateTime(year: DateTime.Now.Year - 27, month: 1, day: 1)}
            };
    }

    private static List<GetAllStudentsResponseDto> GetStudentDtoList()
    {
        return new List<GetAllStudentsResponseDto>
            {
                new (Id: Guid.NewGuid(), Name: "Title1", DateOfBirth: new DateTime(year: DateTime.Now.Year - 25, month: 1, day: 1)),
                new (Id: Guid.NewGuid(), Name: "Title2", DateOfBirth: new DateTime(year: DateTime.Now.Year - 26, month: 1, day: 1)),
                new (Id: Guid.NewGuid(), Name: "Title3", DateOfBirth: new DateTime(year: DateTime.Now.Year - 27, month: 1, day: 1))
            };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<GetAllStudentsResponseDto>>(It.IsAny<IEnumerable<StudentEntity>>()))
            .Returns(GetStudentDtoList());
    }

    private void MockRepositorySetupReturnsData()
    {
        _mockRepositoryWrapper.Setup(x => x.StudentsRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<StudentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StudentEntity>,
            IIncludableQueryable<StudentEntity, object>>>()))
            .ReturnsAsync(GetStudentList());
    }

    private void MockRepositorySetupReturnsNull()
    {
        List<StudentEntity>? list = null;
        _mockRepositoryWrapper.Setup(x => x.StudentsRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<StudentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StudentEntity>,
            IIncludableQueryable<StudentEntity, object>>>()))
            .ReturnsAsync(list);
    }
}
