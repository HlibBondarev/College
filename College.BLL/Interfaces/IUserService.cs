using College.BLL.Services.JwtAuthentication.Models;
using College.DAL.Entities.JwtAuthentication;

namespace College.BLL.Interfaces;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterModel model);
    Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model);
    Task<string> AddRoleAsync(AddRoleModel model);
    Task<AuthenticationModel> RefreshTokenAsync(string token);
    Task<ApplicationUser?> GetById(string id);
    Task<bool> RevokeToken(string token);
}
