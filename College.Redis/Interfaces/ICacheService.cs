namespace College.Redis.Interfaces;

public interface ICacheService
{
    Task<string> ReadAsync(string key);

    Task WriteAsync(
        string key,
        string value,
        TimeSpan? absoluteExpirationRelativeToNowInterval = null,
        TimeSpan? slidingExpirationInterval = null);

    Task RemoveAsync(string key);
}
