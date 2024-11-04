using College.BLL.Services.JwtAuthentication.Models;
using FluentValidation;

namespace College.BLL.Services.JwtAuthentication.Validators;

public class AddRoleModelValidator : AbstractValidator<AddRoleModel>
{
    public const int MINEMAILLENGTH = 6;
    public const int MINPASSLENGTH = 4;
    public const int MINROLELENGTH = 2;
    const string PASSWORDTEMPLATE = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{4,15}$";

    public AddRoleModelValidator()
    {
        RuleFor(rm => rm.Email)
            .NotEmpty()
            .MinimumLength(MINEMAILLENGTH)
            .EmailAddress();

        RuleFor(rm => rm.Password)
            .NotEmpty()
            .MinimumLength(MINPASSLENGTH)
            .Matches(PASSWORDTEMPLATE);

        RuleFor(rm => rm.Role)
            .NotEmpty()
            .MinimumLength(MINROLELENGTH);
    }
}