using FluentValidation.TestHelper;
using College.BLL.DTO.Students;
using College.BLL.MediatR.Student.Create;

namespace College.XUnitTest.ValidationTests.Student;

public class CreateStudentHandlerValidatorTests
{
    private const int MINNAME = 1;
    private const int MAXNAME = 60;
    
    private readonly DateTime _maxDate = new DateTime(year: DateTime.Now.Year - 35, month: 1, day: 1);
    private readonly DateTime _minDate = new DateTime(year: DateTime.Now.Year - 17, month: 1, day: 1);
    private readonly DateTime _acceptableDate = new DateTime(year: DateTime.Now.Year - 25, month: 1, day: 1);


    private readonly CreateStudentCommandValidator _validator;

    public CreateStudentHandlerValidatorTests()
    {
        _validator = new CreateStudentCommandValidator();
    }

    [Theory]
    [InlineData(MINNAME - 1)]
    public void ShouldHaveError_WhenNameLengthIsLessThanAllowed(int number)
    {
        // Arrange
        var command = new CreateStudentCommand(
            Request: new CreateStudentRequestDto(
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
        var command = new CreateStudentCommand(
            Request: new CreateStudentRequestDto(
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
        var command = new CreateStudentCommand(
            Request: new CreateStudentRequestDto(
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
        var command = new CreateStudentCommand(
            Request: new CreateStudentRequestDto(
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
        var command = new CreateStudentCommand(
            Request: new CreateStudentRequestDto(
                Name: new string('a', MINNAME),
                DateOfBirth: _acceptableDate,
                StudentCourses: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Name);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.DateOfBirth);
    }
}
