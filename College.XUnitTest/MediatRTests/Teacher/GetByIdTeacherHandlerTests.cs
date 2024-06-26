using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using College.BLL.DTO.Courses;
using College.BLL.DTO.Teachers;
using College.BLL.Interfaces;
using College.BLL.MediatR.Teacher.GetById;
using College.DAL.Repositories.Interfaces.Base;
using College.BLL.Resources.Errors;
using TeacherEntity = College.DAL.Entities.Teacher;

namespace College.XUnitTest.MediatRTests.Teacher;

public class GetByIdTeacherHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetByIdTeacherHandlerTests()
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
        var query = new GetByIdTeacherQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetById_RepositoryShouldCallGetFirstOrDefaultAsyncOnlyOnce_IfEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsEntity(id);
        MockMapperSetup(id);
        var handler = CreateHandler();
        var query = new GetByIdTeacherQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(
            r =>
            r.TeachersRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
               It.IsAny<Func<IQueryable<TeacherEntity>,
               IIncludableQueryable<TeacherEntity, object>>>()),
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
        var query = new GetByIdTeacherQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        _mockMapper.Verify(
            m => m.Map<GetByIdTeacherResponseDto>(It.IsAny<TeacherEntity>()),
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
        var query = new GetByIdTeacherQuery(id);

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
        var query = new GetByIdTeacherQuery(id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.IsType<GetByIdTeacherResponseDto>(result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnFail_WhenEntityIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockRepositorySetupReturnsNull();
        var handler = CreateHandler();
        var query = new GetByIdTeacherQuery(id);

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
        var query = new GetByIdTeacherQuery(id);

        var expected = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(TeacherEntity).Name,
            id);

        // Act
        var result = await handler.Handle(query, _cancellationToken);
        var actual = result.Errors[0].Message;

        // Assert
        Assert.Equal(expected, actual);
    }

    private GetByIdTeacherHandler CreateHandler()
    {
        return new GetByIdTeacherHandler(
            repositoryWrapper: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void MockMapperSetup(Guid id)
    {
        _mockMapper.Setup(x => x
            .Map<GetByIdTeacherResponseDto>(It.IsAny<TeacherEntity>()))
            .Returns(new GetByIdTeacherResponseDto
            (
                Id: id,
                Name: "Name",
                Degree: "PH Doctor",
                Courses: new List<CourseDto>()
            ));
    }

    private void MockRepositorySetupReturnsEntity(Guid id)
    {
        _mockRepositoryWrapper.Setup(x => x.TeachersRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
               It.IsAny<Func<IQueryable<TeacherEntity>,
               IIncludableQueryable<TeacherEntity, object>>>()))
            .ReturnsAsync(new TeacherEntity { Id = id });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.TeachersRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<TeacherEntity, bool>>>(),
               It.IsAny<Func<IQueryable<TeacherEntity>,
               IIncludableQueryable<TeacherEntity, object>>>()))
            .ReturnsAsync((TeacherEntity?)null);
    }
}
