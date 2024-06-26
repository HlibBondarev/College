using FluentValidation.TestHelper;
using College.BLL.DTO.Teachers;
using College.BLL.MediatR.Teacher.Update;

namespace College.XUnitTest.ValidationTests.Teacher;

public class UpdateTeacherHandlerValidatorTests
{
    private const int MINNAME = 1;
    private const int MINDEGREE = 1;
    private const int MAXNAME = 60;
    private const int MAXDEGREE = 40;

    private readonly UpdateTeacherCommandValidator _validator;

    public UpdateTeacherHandlerValidatorTests()
    {
        _validator = new UpdateTeacherCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpy()
    {
        // Arrange
        var command = new UpdateTeacherCommand(
            Request: new UpdateTeacherRequestDto(
                Id: Guid.Empty,
                Name: new string('a', MINNAME),
                Degree: new string('a', MINDEGREE)
               ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Id);
    }

    [Theory]
    [InlineData(MINNAME - 1)]
    public void ShouldHaveError_WhenNameLengthIsLessThanAllowed(int number)
    {
        // Arrange
        var command = new UpdateTeacherCommand(
            Request: new UpdateTeacherRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', number),
                Degree: new string('a', MINDEGREE + 1)
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Name);
    }

    [Theory]
    [InlineData(MAXNAME + 1)]
    [InlineData(MAXNAME + 10000)]
    public void ShouldHaveError_WhenNameLengthIsIsGreaterThanAllowed(int number)
    {
        // Arrange
        var command = new UpdateTeacherCommand(
            Request: new UpdateTeacherRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', number),
                Degree: new string('a', MINDEGREE)
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Name);
    }

    [Theory]
    [InlineData(MINDEGREE - 1)]
    public void ShouldHaveError_WhenDegreeLengthIsLessThanAllowed(int number)
    {
        // Arrange
        var command = new UpdateTeacherCommand(
            Request: new UpdateTeacherRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', MINNAME),
                Degree: new string('a', number)
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Degree);
    }

    [Theory]
    [InlineData(MAXDEGREE + 1)]
    [InlineData(MAXDEGREE + 10000)]
    public void ShouldHaveError_WhenDegreeLengthIsIsGreaterThanAllowed(int number)
    {
        // Arrange
        var command = new UpdateTeacherCommand(
            Request: new UpdateTeacherRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', MINNAME),
                Degree: new string('a', number)
               ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Degree);
    }

    [Fact]
    public void ShouldNotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateTeacherCommand(
            Request: new UpdateTeacherRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', MINNAME),
                Degree: new string('a', MINDEGREE)
               ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Id);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Name);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Degree);
    }
}

