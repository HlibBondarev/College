﻿namespace College.DAL.Entities.JwtAuthentication;

public class RefreshToken
{
    public int Id { get; set; }
    public string? Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public bool IsActive => Revoked == null && !IsExpired;
}