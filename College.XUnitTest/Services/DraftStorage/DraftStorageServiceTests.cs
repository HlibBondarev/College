using System.Text.Json;
using Microsoft.Extensions.Logging;
using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using College.BLL.DTO.Teachers.Drafts;
using College.BLL.Services.DraftStorage;
using College.Redis.Interfaces;

namespace College.XUnitTest.Services.DraftStorage;

[TestFixture]
public class DraftStorageServiceTests
{
    private const int RANDOMSTRINGSIZE = 50;

    private string key = string.Empty;
    private string cacheKey = string.Empty;

    private Mock<ICacheService>? cacheServiceMock;
    private Mock<ILogger<DraftStorageService<TeacherWithNameDto>>>? loggerMock;
    private DraftStorageService<TeacherWithNameDto>? draftStorageService;

    [SetUp]
    public void SetUp()
    {
        key = new string(new Faker().Random.Chars(min: (char)0, max: (char)127, count: RANDOMSTRINGSIZE));
        cacheKey = GetCacheKey(key, typeof(TeacherWithNameDto));
        cacheServiceMock = new Mock<ICacheService>();
        loggerMock = new Mock<ILogger<DraftStorageService<TeacherWithNameDto>>>();
        draftStorageService = new DraftStorageService<TeacherWithNameDto>(cacheServiceMock.Object, loggerMock.Object);
    }

    [Test]
    public async Task RestoreAsync_WhenDraftExistsInCache_ShouldRestoreAppropriatedEntity()
    {
        // Arrange
        var teacherDraft = GetTeacherWithNameDto();
        cacheServiceMock?.Setup(c => c.ReadAsync(cacheKey))
            .Returns(() => Task.FromResult(JsonSerializer.Serialize(teacherDraft))!)
            .Verifiable(Times.Once);

        // Act
        var result = await draftStorageService!.RestoreAsync(key).ConfigureAwait(false);

        // Assert
        result.Should().BeOfType<TeacherWithNameDto>();
        result!.Name.Should().Be(teacherDraft.Name);
        cacheServiceMock?.VerifyAll();
    }

    [Test]
    public async Task RestoreAsync_WhenDraftIsTeacherWithDegreeDtoAndExistsInCache_ShouldRestoreAppropriatedEntity()
    {
        // Arrange
        var teacherDraft = GetTeacherWithDegreeDto();
        cacheServiceMock?.Setup(c => c.ReadAsync(cacheKey))
            .Returns(() => Task.FromResult(JsonSerializer.Serialize(teacherDraft))!)
            .Verifiable(Times.Once);

        // Act
        var result = await draftStorageService!.RestoreAsync(key).ConfigureAwait(false);

        // Assert      
        result!.Name.Should().Be(teacherDraft.Name);
        (result as TeacherWithDegreeDto)?.Degree.Should().Be(teacherDraft.Degree);
        cacheServiceMock?.VerifyAll();
    }

    [Test]
    public async Task RestoreAsync_WhenDraftIsAbsentInCache_ShouldRestoreDefaultEntity()
    {
        // Arrange
        var teacherDraft = default(TeacherWithNameDto);
        cacheServiceMock?.Setup(c => c.ReadAsync(cacheKey))
            .Returns(() => Task.FromResult(JsonSerializer.Serialize(teacherDraft))!)
            .Verifiable(Times.Once);

        // Act
        var result = await draftStorageService!.RestoreAsync(key).ConfigureAwait(false);

        // Assert
        result.Should().Be(teacherDraft);
        cacheServiceMock?.VerifyAll();
    }

    [Test]
    public void CreateAsync_ShouldCallWriteAsyncOnce()
    {
        // Arrange
        var teacherDraft = GetTeacherWithNameDto();
        var teacherJsonString = JsonSerializer.Serialize(teacherDraft);
        cacheServiceMock?.Setup(c => c.WriteAsync(
            cacheKey,
            teacherJsonString,
            null,
            null))
            .Verifiable(Times.Once);

        // Act
        var result = draftStorageService?.CreateAsync(key, teacherDraft).ConfigureAwait(false);

        // Assert
        cacheServiceMock?.VerifyAll();
    }

    [Test]
    public async Task RemoveAsync_WhenDataExistsInCache_ShouldCallRemoveAsyncOnce()
    {
        // Arrange
        var teacherJsonString = JsonSerializer.Serialize(GetTeacherWithNameDto());
        cacheServiceMock?.Setup(c => c.ReadAsync(cacheKey))
            .Returns(() => Task.FromResult(teacherJsonString)!)
            .Verifiable(Times.Once);
        cacheServiceMock?.Setup(c => c.RemoveAsync(cacheKey))
            .Returns(() => Task.FromResult(teacherJsonString))
            .Verifiable(Times.Once);

        // Act
        await draftStorageService!.RemoveAsync(key).ConfigureAwait(false);

        // Assert
        cacheServiceMock?.VerifyAll();
    }

    [Test]
    public async Task RemoveAsync_WhenDataIsAbsentInCache_ShouldCallRemoveAsyncNever()
    {
        // Arrange
        cacheServiceMock?.Setup(c => c.ReadAsync(cacheKey))
            .Returns(() => Task.FromResult(string.Empty)!).Verifiable(Times.Once);
        cacheServiceMock?.Setup(c => c.RemoveAsync(cacheKey))
            .Returns(() => Task.FromResult(string.Empty)).Verifiable(Times.Never);

        // Act
        await draftStorageService!.RemoveAsync(key).ConfigureAwait(false);

        // Assert
        cacheServiceMock?.VerifyAll();
    }

    private static TeacherWithNameDto GetTeacherWithNameDto()
    {
        var teacherDraft = new Faker<TeacherWithNameDto>()
            .RuleFor(t => t.Name, d => d.Name.LastName());
        return teacherDraft.Generate();
    }

    private static TeacherWithDegreeDto GetTeacherWithDegreeDto()
    {
        var teacherDraft = new Faker<TeacherWithDegreeDto>()
            .RuleFor(t => t.Name, d => d.Name.LastName())
            .RuleFor(t => t.Degree, d => d.Name.JobTitle());
        return teacherDraft.Generate();
    }

    private static string GetCacheKey(string key, Type type)
    {
        return $"{key}_{type.Name}";
    }
}
