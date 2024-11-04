using FluentValidation.TestHelper;
using College.BLL.DTO.Courses;
using College.BLL.MediatR.Course.Create;

namespace College.XUnitTest.ValidationTests.Course;

public class CreateCourseHandlerValidatorTests
{
    private const int MINNAME = 1;
    private const int MINDURATION = 1;
    private const int MAXNAME = 60;
    private const int MAXDURATION = 400;

    private readonly CreateCourseCommandValidator _validator;

    public CreateCourseHandlerValidatorTests()
    {
        _validator = new CreateCourseCommandValidator();
    }

    [Theory]
    [InlineData(MINNAME - 1)]
    public void ShouldHaveError_WhenNameLengthIsLessThanAllowed(int number)
    {
        // Arrange
        var command = new CreateCourseCommand(
            Request: new CreateCourseRequestDto(
                Name: new string('a', number),
                Duration: MINDURATION,
                TeacherId: Guid.NewGuid(),
                CourseStudents: new List<Guid>()
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
        var command = new CreateCourseCommand(
            Request: new CreateCourseRequestDto(
                Name: new string('a', number),
                Duration: MINDURATION,
                TeacherId: Guid.NewGuid(),
                CourseStudents: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Name);
    }

    [Theory]
    [InlineData(MINDURATION - 1)]
    public void ShouldHaveError_WhenDurationIsLessThanAllowed(int number)
    {
        // Arrange
        var command = new CreateCourseCommand(
            Request: new CreateCourseRequestDto(
                Name: new string('a', MINNAME),
                Duration: number,
                TeacherId: Guid.NewGuid(),
                CourseStudents: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Duration);
    }

    [Theory]
    [InlineData(MAXDURATION + 1)]
    [InlineData(MAXDURATION + 10000)]
    public void ShouldHaveError_WhenDurationIsIsGreaterThanAllowed(int number)
    {
        // Arrange
        var command = new CreateCourseCommand(
            Request: new CreateCourseRequestDto(
                Name: new string('a', MINNAME),
                Duration: number,
                TeacherId: Guid.NewGuid(),
                CourseStudents: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.Duration);
    }

    [Fact]
    public void ShouldHaveError_WhenTeacherIdIsEmpty()
    {
        // Arrange
        var command = new CreateCourseCommand(
            Request: new CreateCourseRequestDto(
                Name: new string('a', MINNAME),
                Duration: MINDURATION,
                TeacherId: Guid.Empty,
                CourseStudents: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Request.TeacherId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateCourseCommand(
            Request: new CreateCourseRequestDto(
                Name: new string('a', MINNAME),
                Duration: MINDURATION,
                TeacherId: Guid.NewGuid(),
                CourseStudents: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Name);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Duration);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.TeacherId);
    }
}
