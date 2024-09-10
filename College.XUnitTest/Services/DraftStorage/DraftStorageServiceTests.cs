using College.BLL.DTO.Teachers.Drafts;
using College.BLL.Services.DraftStorage;
using College.BLL.Services.DraftStorage.Interfaces;
using College.Redis.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Text.Json;
using Assert = NUnit.Framework.Assert;

namespace College.XUnitTest.Services.DraftStorage;

[TestFixture]
public class DraftStorageServiceTests
{
    private readonly string jsonStringForTeacherWithDegreeDto = "{\"$type\":\"withDegree\",\"Name\":\"name1\",\"Degree\":\"degree1\"}";

    private Mock<ICacheService> cacheServiceMock;
    private Mock<ILogger<DraftStorageService<TeacherWithNameDto>>> loggerMock;
    private IDraftStorageService<TeacherWithNameDto> draftStorageService;

    [SetUp]
    public void SetUp()
    {
        cacheServiceMock = new Mock<ICacheService>();
        loggerMock = new Mock<ILogger<DraftStorageService<TeacherWithNameDto>>>();
        draftStorageService = new DraftStorageService<TeacherWithNameDto>(cacheServiceMock.Object, loggerMock.Object);
    }

    [Test]
    public async Task RestoreAsync_WhenDraftExistsInCache_ShouldRestoreAppropriatedEntity()
    {
        // Arrange
        var teacherDraft = new TeacherWithNameDto()
        {
            Name = "name"
        };
        var expected = new Dictionary<string, string>()
            {
                {"ExpectedKey", "{\"Name\":\"name\"}"},
            };

        cacheServiceMock.Setup(c => c.ReadAsync(It.IsAny<string>()))
            .Returns(() => Task.FromResult(expected["ExpectedKey"]));

        // Act
        var result = await draftStorageService.RestoreAsync("ExpectedKey").ConfigureAwait(false);

        // Assert
        cacheServiceMock.Verify(
            c => c.ReadAsync(It.IsAny<string>()),
            Times.Once);
        result.Should().BeOfType<TeacherWithNameDto>();
        result?.Name.Should().Be(teacherDraft.Name);
    }

    [Test]
    public async Task RestoreAsync_WhenDraftIsTeacherWithDegreeDtoAndExistsInCache_ShouldRestoreAppropriatedEntity()
    {
        var teacherDraft = FakeDraft(jsonStringForTeacherWithDegreeDto);

        var expected = new Dictionary<string, string>()
            {
                {"ExpectedKey", jsonStringForTeacherWithDegreeDto },
            };

        cacheServiceMock.Setup(c => c.ReadAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(expected["ExpectedKey"]));

        // Act
        var result = await draftStorageService.RestoreAsync("ExpectedKey").ConfigureAwait(false);

        // Assert
        cacheServiceMock.Verify(
        c => c.ReadAsync(It.IsAny<string>()),
        Times.Once);
        result?.Name.Should().Be(teacherDraft.Name);
        (result as TeacherWithDegreeDto)?.Degree.Should().Be((teacherDraft as TeacherWithDegreeDto)?.Degree);
    }

    [Test]
    public async Task RestoreAsync_WhenDraftIsAbsentInCache_ShouldRestoreDefaultEntity()
    {
        // Arrange
        var expected = new Dictionary<string, string>()
            {
                {"ExpectedKey", null},
            };
        cacheServiceMock.Setup(c => c.ReadAsync(It.IsAny<string>()))
            .Returns(() => Task.FromResult(expected["ExpectedKey"]));

        // Act
        var result = await draftStorageService.RestoreAsync("ExpectedKey").ConfigureAwait(false);

        // Assert
        cacheServiceMock.Verify(
            c => c.ReadAsync(It.IsAny<string>()),
            Times.Once);
        result.Should().BeNull();
    }

    [Test]
    public void CreateAsync_ShouldCallWriteAsyncOnce()
    {
        // Arrange
        var teacherDraft = new TeacherWithNameDto()
        {
            Name = "name1"
        };

        cacheServiceMock.Setup(c => c.WriteAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            null,
            null));

        // Act
        var result = draftStorageService.CreateAsync("ExpectedKey", teacherDraft).ConfigureAwait(false);

        // Assert
        cacheServiceMock.Verify(
            c => c.WriteAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            null,
            null),
            Times.Once);
    }

    [Test]
    public async Task RemoveAsync_WhenDataExistsInCache_ShouldCallRemoveAsyncOnce()
    {
        // Arrange
        var expected = new Dictionary<string, string>()
                {
                    {"ExpectedKey", "ExpectedValue"},
                };
        cacheServiceMock.Setup(c => c.ReadAsync(It.IsAny<string>()))
            .Returns(() => Task.FromResult(expected["ExpectedKey"]));

        // Act
        await draftStorageService.RemoveAsync("ExpectedKey").ConfigureAwait(false);

        // Assert
        cacheServiceMock.Verify(
            c => c.ReadAsync(It.IsAny<string>()),
            Times.Once);
        cacheServiceMock.Verify(
            c => c.RemoveAsync(It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task RemoveAsync_WhenDataIsAbsentInCache_ShouldCallRemoveAsyncNever()
    {
        // Arrange
        var expected = new Dictionary<string, string>()
                {
                    {"ExpectedKey", null},
                };
        cacheServiceMock.Setup(c => c.ReadAsync(It.IsAny<string>()))
            .Returns(() => Task.FromResult(expected["ExpectedKey"]));

        // Act && Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await draftStorageService.RemoveAsync("ExpectedKey").ConfigureAwait(false));
        cacheServiceMock.Verify(
            c => c.ReadAsync(It.IsAny<string>()),
            Times.Once);
        cacheServiceMock.Verify(
            c => c.RemoveAsync(It.IsAny<string>()),
            Times.Never);
    }

    private TeacherWithNameDto FakeDraft(string jsonString)
    {
        return JsonSerializer.Deserialize<TeacherWithNameDto>(jsonString);
    }
}
