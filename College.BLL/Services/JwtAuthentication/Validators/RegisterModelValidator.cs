using FluentValidation;
using College.BLL.Services.JwtAuthentication.Models;

namespace College.BLL.Services.JwtAuthentication.Validators;

public class RegisterModelValidator : AbstractValidator<RegisterModel>
{
    const int MINFIRSTNAMELENGTH = 2;
    const int MINLASTNAMELENGTH = 2;
    const int MINUSERNAMELENGTH = 2;
    const int MINEMAILLENGTH = 6;
    const int MINPASSLENGTH = 4;
    const int MAXFIRSTNAMELENGTH = 25;
    const int MAXLASTNAMELENGTH = 25;
    const int MAXUSERNAMELENGTH = 25;
    const string PASSWORDTEMPLATE = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{4,15}$";

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
            .MinimumLength(MINPASSLENGTH)
            .Matches(PASSWORDTEMPLATE);
    }
}
