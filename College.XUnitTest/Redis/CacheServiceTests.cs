using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using College.Redis.Interfaces;
using College.Redis.Models;
using College.Redis;

namespace College.XUnitTest.Redis;

[TestFixture]
public class CacheServiceTests
{
    private Mock<IDistributedCache> distributedCacheMock;
    private Mock<IOptions<RedisConfig>> redisConfigMock;
    private ICacheService cacheService;

    [SetUp]
    public void SetUp()
    {
        distributedCacheMock = new Mock<IDistributedCache>();
        redisConfigMock = new Mock<IOptions<RedisConfig>>();
        redisConfigMock.Setup(c => c.Value).Returns(new RedisConfig
        {
            Enabled = true,
            AbsoluteExpirationRelativeToNowInterval = TimeSpan.FromMinutes(1),
            SlidingExpirationInterval = TimeSpan.FromMinutes(1),
        });
        cacheService = new CacheService(distributedCacheMock.Object, redisConfigMock.Object);
    }

    [Test]
    public async Task RemoveAsync_ShouldCallCacheRemoveOnce()
    {
        // Arrange & Act
        await cacheService.RemoveAsync("Example");

        // Assert
        distributedCacheMock.Verify(
            c => c.Remove("Example"),
            Times.Once);
    }

    [Test]
    public async Task ReadAsync_WhenDataExistsInCacheAndNotExpired_ShouldReturnData()
    {
        // Arrange
        var expected = new Dictionary<string, string>()
        {
            {"ExpectedKey", "ExpectedValue"},
        };
        distributedCacheMock.Setup(c => c.Get(It.IsAny<string>()))
            .Returns(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expected)));

        // Act
        var result = await cacheService.ReadAsync("ExpectedKey");

        // Assert
        result.Should().Contain("ExpectedValue");
        distributedCacheMock.Verify(
            c => c.Get(It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task ReadAsync_WhenDataNotExistsOrExpired_ShouldReturnNull()
    {
        // Arrange
        var expected = new Dictionary<string, string>()
        {
            {"ExpectedKey", null},
        };
        distributedCacheMock.Setup(c => c.Get(It.IsAny<string>()))
            .Returns(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expected)));

        // Act
        var result = await cacheService.ReadAsync("ExpectedKey");

        // Assert
        result.Should().Contain("{\"ExpectedKey\":null}");
        distributedCacheMock.Verify(
            c => c.Get(It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task WriteAsync_ShouldCallCacheSetOnce()
    {
        // Arrange & Act
        await cacheService.WriteAsync("ExpectedKey", "ExpectedValue");

        // Assert
        distributedCacheMock.Verify(
            c => c.Set(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>()),
            Times.Once);
    }
}
