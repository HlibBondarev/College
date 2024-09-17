using FluentValidation.TestHelper;
using College.BLL.DTO.Courses;
using College.BLL.MediatR.Course.Update;

namespace College.XUnitTest.ValidationTests.Course;

public class UpdateCourseHandlerValidatorTests
{
    private const int MINNAME = 1;
    private const int MINDURATION = 1;
    private const int MAXNAME = 60;
    private const int MAXDURATION = 400;

    private readonly UpdateCourseCommandValidator _validator;

    public UpdateCourseHandlerValidatorTests()
    {
        _validator = new UpdateCourseCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new UpdateCourseCommand(
            Request: new UpdateCourseRequestDto(
                Id:  Guid.Empty,
                Name: new string('a', MINNAME),
                Duration: MINDURATION,
                TeacherId: Guid.NewGuid(),
                CourseStudents: new List<Guid>()
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
        var command = new UpdateCourseCommand(
            Request: new UpdateCourseRequestDto(
                Id: Guid.NewGuid(),
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
        var command = new UpdateCourseCommand(
            Request: new UpdateCourseRequestDto(
                Id: Guid.NewGuid(),
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
        var command = new UpdateCourseCommand(
            Request: new UpdateCourseRequestDto(
                Id: Guid.NewGuid(),
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
        var command = new UpdateCourseCommand(
            Request: new UpdateCourseRequestDto(
                Id: Guid.NewGuid(),
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
        var command = new UpdateCourseCommand(
            Request: new UpdateCourseRequestDto(
                Id: Guid.NewGuid(),
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
        var command = new UpdateCourseCommand(
            Request: new UpdateCourseRequestDto(
                Id: Guid.NewGuid(),
                Name: new string('a', MINNAME),
                Duration: MINDURATION,
                TeacherId: Guid.NewGuid(),
                CourseStudents: new List<Guid>()
                ));

        // Act
        var validationResult = _validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Id);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Name);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.Duration);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Request.TeacherId);
    }
}
