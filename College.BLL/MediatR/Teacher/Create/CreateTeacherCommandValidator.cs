using FluentValidation;

namespace College.BLL.MediatR.Teacher.Create;

public class CreateTeacherCommandValidator : AbstractValidator<CreateTeacherCommand>
{
    public CreateTeacherCommandValidator()
    {
        RuleFor(c => c.Request.Name)
            .NotEmpty()
            .MaximumLength(60)
            .WithName("Name");

        RuleFor(c => c.Request.Degree)
            .NotEmpty()
            .MaximumLength(40)
            .WithName("Degree");
    }
}
