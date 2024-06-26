using FluentValidation;

namespace College.BLL.MediatR.Teacher.Delete;

public class DeleteTeacherCommandValidator : AbstractValidator<DeleteTeacherCommand>
{
    public DeleteTeacherCommandValidator()
    {
        RuleFor(c => c.Request.Id)
            .NotEmpty()
            .WithName("Id");
    }
}
