using FluentValidation;

namespace College.BLL.MediatR.Teacher.Delete;

public class DeleteTeacherCommandDtoValidator : AbstractValidator<DeleteTeacherCommand>
{
    public DeleteTeacherCommandDtoValidator()
    {
        RuleFor(c => c.Request.Id).NotEmpty().WithName("Id");
    }
}
