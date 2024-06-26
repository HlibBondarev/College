using FluentValidation.TestHelper;
using College.BLL.DTO.Teachers;
using College.BLL.MediatR.Teacher.Delete;

namespace College.XUnitTest.ValidationTests.Teacher;

public class DeleteTeacherHandlerValidatorTests
{
    private readonly DeleteTeacherCommandValidator _validator;

    public DeleteTeacherHandlerValidatorTests()
    {
        _validator = new DeleteTeacherCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpy()
    {
        // Arrange
        var command = new DeleteTeacherCommand(
            Request: new DeleteTeacherRequestDto(
                Id: Guid.Empty
               ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteTeacherCommand(
            Request: new DeleteTeacherRequestDto(
                Id: Guid.NewGuid()));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Id);
    }
}