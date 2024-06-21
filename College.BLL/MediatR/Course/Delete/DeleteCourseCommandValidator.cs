using FluentValidation;

namespace College.BLL.MediatR.Course.Delete;

public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(c => c.Request.Id).NotEmpty().WithName("Id");
    }
}