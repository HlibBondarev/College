using College.BLL.MediatR.Teacher.Update;
using FluentValidation;

namespace College.BLL.MediatR.Student.Update;
public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(c => c.Request.Id)
            .NotEmpty()
            .WithName("Id");

        RuleFor(c => c.Request.Name)
            .NotEmpty()
            .MaximumLength(60)
            .WithName("Name");

        RuleFor(c => c.Request.DateOfBirth)
            .NotEmpty()
            .GreaterThan(new DateTime(year: DateTime.Now.Year - 35, month: 1, day: 1))
            .LessThan(new DateTime(year: DateTime.Now.Year - 17, month: 1, day: 1))
            .WithName("DateOfBirth");
    }
}
