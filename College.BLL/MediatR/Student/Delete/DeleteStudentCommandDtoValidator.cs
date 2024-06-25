using FluentValidation;

namespace College.BLL.MediatR.Student.Delete;

public class DeleteStudentCommandDtoValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandDtoValidator()
    {
        RuleFor(c => c.Request.Id)
            .NotEmpty()
            .WithName("Id");
    }
}
