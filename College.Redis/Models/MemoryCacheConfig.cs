using System.ComponentModel.DataAnnotations;

namespace College.Redis.Models;

public class MemoryCacheConfig
{
    public const string Name = "MemoryCache";

    [Required]
    public TimeSpan AbsoluteExpirationRelativeToNowInterval { get; set; }

    [Required]
    public TimeSpan SlidingExpirationInterval { get; set; }
}
