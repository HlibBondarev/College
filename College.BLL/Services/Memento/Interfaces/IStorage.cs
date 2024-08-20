using College.Redis;

namespace College.BLL.Services.Memento.Interfaces;

public interface IStorage
{
    KeyValuePair<string, string?> this[string key] { get; set; }
    IRedisCacheService RedisCacheService {  set; }
}