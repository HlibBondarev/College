using FluentValidation.TestHelper;
using College.BLL.DTO.Students;
using College.BLL.MediatR.Student.Update;

namespace College.XUnitTest.ValidationTests.Student;

public class UpdateStudentHandlerValidatorTests
{
    private const int MINNAME = 1;
    private const int MAXNAME = 60;

    private readonly DateTime _maxDate = new DateTime(year: DateTime.Now.Year - 35, month: 1, day: 1);
    private readonly DateTime _minDate = new DateTime(year: DateTime.Now.Year - 17, month: 1, day: 1);
    private readonly DateTime _acceptableDate = new DateTime(year: DateTime.Now.Year - 25, month: 1, day: 1);


    private readonly UpdateStudentCommandValidator _validator;

    public UpdateStudentHandlerValidatorTests()
    {
        _validator = new UpdateStudentCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpy()
    {
        // Arrange
        var command = new UpdateStudentCommand(
            Request: new UpdateStudentRequestDto(
                Id: Guid.Empty,
                Name: new string('a', MINNAME),
                DateOfBirth: _acceptableDate,
                StudentCourses: new List<Guid>() 
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
        var command = new UpdateStudentCommand(
            Request: new UpdateStudentRequestDto(
                Id: Guid.NewGuid(), 
                Name: new string('a', number),
                DateOfBirth: _acceptableDate,
                StudentCourses: new List<Guid>()
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
        var command = new UpdateStudentCommand(
            Request: new UpdateStudentRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', number),
                DateOfBirth: _acceptableDate,
                StudentCourses: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Name);
    }

    [Fact]
    public void ShouldHaveError_WhenDateOfBirthIsIsGreaterThanAllowed()
    {
        // Arrange
        var command = new UpdateStudentCommand(
            Request: new UpdateStudentRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', MINNAME),
                DateOfBirth: _maxDate,
                StudentCourses: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.DateOfBirth);
    }

    [Fact]
    public void ShouldHaveError_WhenDateOfBirthIsIsLessThanAllowed()
    {
        // Arrange
        var command = new UpdateStudentCommand(
            Request: new UpdateStudentRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', MINNAME),
                DateOfBirth: _minDate,
                StudentCourses: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.DateOfBirth);
    }

    [Fact]
    public void ShouldNotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateStudentCommand(
            Request: new UpdateStudentRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', MINNAME),
                DateOfBirth: _acceptableDate,
                StudentCourses: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Id);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Name);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.DateOfBirth);
    }
}
