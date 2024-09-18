using System.Text;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using College.Redis;
using College.Redis.Interfaces;
using College.Redis.Models;

namespace College.XUnitTest.Redis;

[TestFixture]
public class CacheServiceTests
{
    private const int RANDOMSTRINGSIZE = 50;

    private string expectedValue = string.Empty;
    private string expectedKey = string.Empty;
    private Mock<IDistributedCache>? distributedCacheMock;
    private Mock<IOptions<RedisConfig>>? redisConfigMock;
    private CacheService? cacheService;

    [SetUp]
    public void SetUp()
    {
        expectedValue = new string(new Faker().Random.Chars(min: (char)0, max: (char)127, count: RANDOMSTRINGSIZE));
        expectedKey = new string(new Faker().Random.Chars(min: (char)0, max: (char)127, count: RANDOMSTRINGSIZE));
        distributedCacheMock = new Mock<IDistributedCache>();
        redisConfigMock = new Mock<IOptions<RedisConfig>>();
        redisConfigMock.Setup(c => c.Value).Returns(new RedisConfig
        {
            Server = "server",
            Password = "password",
            Enabled = true,
            AbsoluteExpirationRelativeToNowInterval = TimeSpan.FromMinutes(1),
            SlidingExpirationInterval = TimeSpan.FromMinutes(1),
        });
        cacheService = new CacheService(distributedCacheMock.Object, redisConfigMock.Object);
    }

    [Test]
    public async Task RemoveAsync_ShouldCallRemoveAsyncOnce()
    {

        // Arrange
        distributedCacheMock!.Setup(c => c.Remove(expectedKey))
            .Verifiable(Times.Once);

        // Act
        await cacheService!.RemoveAsync(expectedKey);

        // Assert
        distributedCacheMock.VerifyAll();
    }

    [Test]
    public async Task ReadAsync_WhenDataExistsInCacheAndNotExpired_ShouldReturnData()
    {
        // Arrange
        distributedCacheMock!.Setup(c => c.Get(expectedKey))
            .Returns(Encoding.UTF8.GetBytes(expectedValue))
            .Verifiable(Times.Once);

        // Act
        var result = await cacheService!.ReadAsync(expectedKey);

        // Assert
        result.Should().Be(expectedValue);
        distributedCacheMock.VerifyAll();
    }

    [Test]
    public async Task ReadAsync_WhenDataNotExistsOrExpired_ShouldReturnNull()
    {
        // Arrange
        distributedCacheMock!.Setup(c => c.Get(expectedKey))
            .Returns(Encoding.UTF8.GetBytes(string.Empty))
            .Verifiable(Times.Once);

        // Act
        var result = await cacheService!.ReadAsync(expectedKey);

        // Assert
        result.Should().Be(string.Empty);
        distributedCacheMock.VerifyAll();
    }

    [Test]
    public async Task WriteAsync_ShouldCallCacheSetOnce()
    {
        // Arrange
        distributedCacheMock!.Setup(c => c.Set(expectedKey, Encoding.UTF8.GetBytes(expectedValue), It.IsAny<DistributedCacheEntryOptions>()))
            .Verifiable(Times.Once);

        // Act
        await cacheService!.WriteAsync(expectedKey, expectedValue);

        // Assert
        distributedCacheMock.VerifyAll();
    }
}
