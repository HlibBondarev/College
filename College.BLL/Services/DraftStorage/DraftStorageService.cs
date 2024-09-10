using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using College.BLL.Services.DraftStorage.Interfaces;
using College.Redis.Interfaces;

namespace College.BLL.Services.DraftStorage;

/// <summary>
/// Implements the IDraftStorageService{T} interface to store an entity draft of type T in a cache.
/// </summary>
/// <typeparam name="T">T is the entity draft type that should be stored in the cache.</typeparam>
public class DraftStorageService<T> : IDraftStorageService<T>
{
    private readonly ICacheService cacheService;
    private readonly ILogger<DraftStorageService<T>> logger;

    /// <summary>Initializes a new instance of the <see cref="DraftStorageService{T}" /> class.</summary>
    /// <param name="cacheService">The cache service.</param>
    /// <param name="logger">The logger.</param>
    public DraftStorageService(
        ICacheService cacheService,
        ILogger<DraftStorageService<T>> logger)
    {
        this.cacheService = cacheService;
        this.logger = logger;
    }

    /// <summary>Restores the entity draft of T type entity.</summary>
    /// <param name="key">The key.</param>
    /// <returns> Representing the asynchronous operation with result of T type.</returns>
    public async Task<T?> RestoreAsync([NotNull] string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        var draftValue = await cacheService.ReadAsync(GetKey(key)).ConfigureAwait(false);

        if (draftValue is null)
        {
            logger.LogInformation($"The {typeof(T).Name} draft for User with key = {key} was not found in the cache.");
            return default;
        }

        return JsonSerializer.Deserialize<T>(draftValue);
    }

    /// <summary>Creates the entity draft of T type in the cache.</summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// Representing the asynchronous operation - creating the entity draft of T type in the cache.
    /// </returns>
    public async Task CreateAsync([NotNull] string key, [NotNull] T value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(value);

        await cacheService.WriteAsync(GetKey(key), JsonSerializer.Serialize(value)).ConfigureAwait(false);
    }

    /// <summary>Asynchronously removes an entity draft from the cache.</summary>
    /// <param name="key">The key.</param>
    /// <returns>Representation of an asynchronous operation - removing an entity draft from the cache.</returns>
    public async Task RemoveAsync([NotNull] string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        var draftKey = GetKey(key);
        var valueToRemove = await cacheService.ReadAsync(draftKey);

        if (valueToRemove == null)
        {
            logger.LogError($"The {typeof(T)} draft with key = {draftKey} was not found in the cache.");
            throw new InvalidOperationException($"The {typeof(T).Name} draft with key = {draftKey} was not found in the cache.");
        }

        logger.LogInformation($"Start removing the {typeof(T).Name} draft with key = {draftKey} from cache.");
        await cacheService.RemoveAsync(draftKey).ConfigureAwait(false);
    }

    private static string GetKey(string key)
    {
        return $"{key}_{typeof(T).Name}";
    }
}
