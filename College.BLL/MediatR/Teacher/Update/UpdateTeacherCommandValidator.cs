using FluentValidation;

namespace College.BLL.MediatR.Teacher.Update;

public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
{
    public UpdateTeacherCommandValidator()
    {
        RuleFor(c => c.Request.Id)
            .NotEmpty()
            .WithName("Id");

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