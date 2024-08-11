using FluentValidation;
using College.BLL.Services.JwtAuthentication.Models;

namespace College.BLL.Services.JwtAuthentication.Validators;

public class RegisterModelValidator : AbstractValidator<RegisterModel>
{
    public const int MINFIRSTNAMELENGTH = 2;
    public const int MINLASTNAMELENGTH = 2;
    public const int MINUSERNAMELENGTH = 2;
    public const int MINEMAILLENGTH = 6;
    public const int MINPASSLENGTH = 4;
    public const int MAXFIRSTNAMELENGTH = 25;
    public const int MAXLASTNAMELENGTH = 25;
    public const int MAXUSERNAMELENGTH = 25;

    public RegisterModelValidator()
    {
        RuleFor(rm => rm.FirstName)
            .NotEmpty()
            .MinimumLength(MINFIRSTNAMELENGTH)
            .MaximumLength(MAXFIRSTNAMELENGTH);

        RuleFor(rm => rm.LastName)
            .NotEmpty()
            .MinimumLength(MINLASTNAMELENGTH)
            .MaximumLength(MAXLASTNAMELENGTH);

        RuleFor(rm => rm.UserName)
            .NotEmpty()
            .MinimumLength(MINUSERNAMELENGTH)
            .MaximumLength(MAXUSERNAMELENGTH);

        RuleFor(rm => rm.Email)
            .NotEmpty()
            .MinimumLength(MINEMAILLENGTH)
            .EmailAddress();

        RuleFor(rm => rm.Password)
            .NotEmpty()
            .MinimumLength(MINPASSLENGTH);
    }
}
