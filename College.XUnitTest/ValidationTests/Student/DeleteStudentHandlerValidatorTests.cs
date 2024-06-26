using FluentValidation.TestHelper;
using College.BLL.DTO.Students;
using College.BLL.MediatR.Student.Delete;

namespace College.XUnitTest.ValidationTests.Student;

public class DeleteStudentHandlerValidatorTests
{
    private readonly DeleteStudentCommandValidator _validator;

    public DeleteStudentHandlerValidatorTests()
    {
        _validator = new DeleteStudentCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpy()
    {
        // Arrange
        var command = new DeleteStudentCommand(
            Request: new DeleteStudentRequestDto(
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
        var command = new DeleteStudentCommand(
            Request: new DeleteStudentRequestDto(
                Id: Guid.NewGuid()));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Id);
    }
}