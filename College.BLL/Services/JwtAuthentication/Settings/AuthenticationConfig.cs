namespace College.BLL.Services.JwtAuthentication.Settings;

public class AuthenticationConfig
{
    public const string Name = "Authentication";

    public List<string> Roles { get; set; }= new List<string>();
}
