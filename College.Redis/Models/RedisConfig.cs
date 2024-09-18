using System.ComponentModel.DataAnnotations;

namespace College.Redis.Models;

public class RedisConfig
{
    public const string Name = "Redis";

    public bool Enabled { get; set; }

    [Required]
    public required string Server { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Port must be greater then 0.")]
    public int Port { get; set; }

    [Required]
    public required TimeSpan AbsoluteExpirationRelativeToNowInterval { get; set; }

    [Required]
    public required TimeSpan SlidingExpirationInterval { get; set; }

    [Required]
    public TimeSpan CheckAlivePollingInterval { get; set; }
}
