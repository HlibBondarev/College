using College.BLL.Services.JwtAuthentication.Models;

namespace College.BLL.Interfaces;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterModel model);
    Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model);
    Task<string> AddRoleAsync(AddRoleModel model);
}
