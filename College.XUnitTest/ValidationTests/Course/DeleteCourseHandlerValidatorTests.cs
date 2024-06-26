using FluentValidation.TestHelper;
using College.BLL.DTO.Courses;
using College.BLL.MediatR.Course.Delete;

namespace College.XUnitTest.ValidationTests.Course;

public class DeleteCourseHandlerValidatorTests
{
    private readonly DeleteCourseCommandValidator _validator;

    public DeleteCourseHandlerValidatorTests()
    {
        _validator = new DeleteCourseCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpy()
    {
        // Arrange
        var command = new DeleteCourseCommand(
            Request: new DeleteCourseRequestDto(
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
        var command = new DeleteCourseCommand(
            Request: new DeleteCourseRequestDto(
                Id: Guid.NewGuid()));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Id);
    }
}
