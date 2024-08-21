using College.BLL.Services.Memento.Interfaces;
using College.Redis;
using Newtonsoft.Json.Linq;

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

    public KeyValuePair<string, string?> this[string key]
    {
        get
        {
            if (_redisCacheService is not null)
            {
                Task<string?> task = Task.Run(async () => await _redisCacheService.GetValueFromRedisCacheAsync(key));
                return new KeyValuePair<string, string?>(key, task.Result);
            }
            return new KeyValuePair<string, string?>();
        }
        set
        {
            if (_redisCacheService is not null)
            {
                Task.Run(async () => await _redisCacheService.SetValueToRedisCacheAsync(key, value.Value ?? string.Empty));
            }
        }
    }

    public void RemoveMemento(string key)
    {
        if (_redisCacheService is not null)
        {
            Task.Run(async () => await _redisCacheService.RemoveValueFromRedisCacheAsync(key));
        }
    }
}
