using FluentValidation;

namespace College.BLL.MediatR.Course.Update;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(c => c.Request.Id)
            .NotEmpty()
            .WithName("Id");

        RuleFor(c => c.Request.Name)
            .NotEmpty()
            .MaximumLength(60)
            .WithName("Name");

        RuleFor(c => c.Request.Duration)
            .NotEmpty()
            .GreaterThan(0)
            .WithName("Duration");

        RuleFor(c => c.Request.TeacherId)
            .NotEmpty()
            .WithName("TeacherId");
    }
}