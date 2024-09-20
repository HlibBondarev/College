using College.BLL.Services.JwtAuthentication.Models;
using FluentValidation;

namespace College.BLL.Services.JwtAuthentication.Validators;

public class TokenRequestModelValidator : AbstractValidator<TokenRequestModel>
{
    public const int MINEMAILLENGTH = 6;
    public const int MINPASSLENGTH = 4;
    const string PASSWORDTEMPLATE = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{4,15}$";

    public TokenRequestModelValidator()
    {
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

