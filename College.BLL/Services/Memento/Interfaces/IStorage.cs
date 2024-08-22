using College.Redis;

namespace College.BLL.Services.Memento.Interfaces;

public interface IStorage
{
    IRedisCacheService RedisCacheService { set; }
    Task SetMementoValueAsync(KeyValuePair<string, string?> keyValue);
    Task<KeyValuePair<string, string?>> GetMementoValueAsync(string key);
    Task RemoveMementoAsync(string key);
    //KeyValuePair<string, string?> this[string key] { get; set; }
}