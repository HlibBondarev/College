using System.ComponentModel.DataAnnotations;

namespace College.BLL.Services.JwtAuthentication.Models;

public class TokenRequestModel
{
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}