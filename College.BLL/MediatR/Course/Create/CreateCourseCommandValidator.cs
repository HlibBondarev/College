using FluentValidation;

namespace College.BLL.MediatR.Course.Create;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(c => c.Request.Name).NotEmpty().MaximumLength(60).WithName("Name");
        RuleFor(c => c.Request.Duration).NotEmpty().GreaterThan(0).WithName("Duration");
    }
}