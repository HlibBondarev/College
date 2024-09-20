using System.ComponentModel.DataAnnotations;

namespace College.BLL.Services.JwtAuthentication.Models;

public class TokenRequestModel
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}