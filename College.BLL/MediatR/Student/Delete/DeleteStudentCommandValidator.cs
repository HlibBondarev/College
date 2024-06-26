using FluentValidation;

namespace College.BLL.MediatR.Student.Delete;

public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandValidator()
    {
        RuleFor(c => c.Request.Id)
            .NotEmpty()
            .WithName("Id");
    }
}
