using College.BLL.Services.Memento.Interfaces;
using College.Redis;

namespace College.BLL.Services.Memento;

public class Storage : IStorage
{
    private IRedisCacheService? _redisCacheService;

    public IRedisCacheService RedisCacheService
    {
        set
        {
            _redisCacheService = value;
        }
    }

    public async Task SetMementoValueAsync(KeyValuePair<string, string?> keyValue)
    {
        if (_redisCacheService is not null)
        {
            await _redisCacheService.SetValueToRedisCacheAsync(keyValue.Key, keyValue.Value ?? string.Empty);
        }
    }

    public async Task<KeyValuePair<string, string?>> GetMementoValueAsync(string key)
    {
        if (_redisCacheService is not null)
        {
            var value = await _redisCacheService.GetValueFromRedisCacheAsync(key);
            return new KeyValuePair<string, string?>(key, value);
        }
        return new KeyValuePair<string, string?>();
    }

    public async Task RemoveMementoAsync(string key)
    {
        if (_redisCacheService is not null)
        {
            await _redisCacheService.RemoveValueFromRedisCacheAsync(key);
        }
    }

    //public KeyValuePair<string, string?> this[string key]
    //{
    //    get
    //    {
    //        if (_redisCacheService is not null)
    //        {
    //            Task<string?> task = Task.Run(async () => await _redisCacheService.GetValueFromRedisCacheAsync(key));
    //            return new KeyValuePair<string, string?>(key, task.Result);
    //        }
    //        return new KeyValuePair<string, string?>();
    //    }
    //    set
    //    {
    //        if (_redisCacheService is not null)
    //        {
    //            Task.Run(async () => await _redisCacheService.SetValueToRedisCacheAsync(key, value.Value ?? string.Empty));
    //        }
    //    }
    //}
}
