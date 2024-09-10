using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;
using College.Redis.Interfaces;
using College.Redis.Models;

namespace College.Redis;

public class CacheService : ICacheService, IDisposable
{
    private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
    private readonly IDistributedCache cache;
    private readonly RedisConfig redisConfig;

    private bool isWorking = true;
    private readonly bool isEnabled = false;

    private readonly object lockObject = new object();

    private bool isDisposed;

    public CacheService(
        IDistributedCache cache,
        [NotNull] IOptions<RedisConfig> redisConfig
    )
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(redisConfig.Value);

        this.cache = cache;

        try
        {
            this.redisConfig = redisConfig.Value;
            isEnabled = this.redisConfig.Enabled;
        }
        catch (OptionsValidationException)
        {
            isEnabled = false;
        }
    }

    public Task RemoveAsync(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        return ExecuteRedisMethod(() =>
            {
                cacheLock.EnterWriteLock();
                try
                {
                    cache.Remove(key);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            });
    }

    public async Task<string?> ReadAsync(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        string? returnValue = string.Empty;

        await ExecuteRedisMethod(() =>
        {
            cacheLock.EnterReadLock();
            try
            {
                returnValue = cache.GetString(key);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        });

        return returnValue;
    }

    public async Task WriteAsync(string key, string value, TimeSpan? absoluteExpirationRelativeToNowInterval = null, TimeSpan? slidingExpirationInterval = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentException.ThrowIfNullOrEmpty(value);

        await ExecuteRedisMethod(() =>
        {
            cacheLock.EnterWriteLock();
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNowInterval ?? redisConfig.AbsoluteExpirationRelativeToNowInterval,
                    SlidingExpiration = slidingExpirationInterval ?? redisConfig.SlidingExpirationInterval,
                };

                cache.SetString(key, value, options);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        });
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !isDisposed)
        {
            cacheLock?.Dispose();
            isDisposed = true;
        }
    }

    private async Task RedisIsBrokenStartCheck()
    {
        if (Monitor.TryEnter(lockObject))
        {
            isWorking = false;

            try
            {
                while (!isWorking)
                {
                    try
                    {
                        cache.GetString("_");
                        isWorking = true;
                    }
                    catch (RedisConnectionException)
                    {
                        await Task.Delay(redisConfig.CheckAlivePollingInterval);
                    }
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
        }
    }

    private async Task ExecuteRedisMethod(Action redisMethod)
    {
        if (isEnabled && isWorking)
        {
            try
            {
                await Task.Run(redisMethod);
            }
            catch (RedisConnectionException)
            {
                _ = Task.Run(RedisIsBrokenStartCheck);
            }
        }
    }
}