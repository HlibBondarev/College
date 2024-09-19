using System.Security.Claims;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using College.BLL.Common;
using College.BLL.DTO.Teachers.Drafts;
using College.BLL.Services.DraftStorage.Interfaces;
using College.WebApi.Controllers;

namespace College.XUnitTest.Controllers;

[TestFixture]
public class TeacherDraftStorageControllerTests
{
    private const int RANDOMSTRINGSIZE = 50;

    private string key = string.Empty;
    private Mock<IDraftStorageService<TeacherWithNameDto>>? draftStorageService;
    private TeacherDraftStorageController? controller;
    private ClaimsPrincipal? user;
    private TeacherWithNameDto? draft;

    [SetUp]
    public void Setup()
    {
        draft = GetTeacherWithNameDtoDraft();
        draftStorageService = new Mock<IDraftStorageService<TeacherWithNameDto>>();
        controller = new TeacherDraftStorageController(draftStorageService.Object);
        user = new ClaimsPrincipal(new ClaimsIdentity());
        key = GettingUserProperties.GetUserId(user);
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
    }

    [Test]
    public async Task StoreDraft_WhenModelIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        draftStorageService?.Setup(ds => ds.CreateAsync(key, draft!))
            .Verifiable(Times.Once);
        var resultValue = $"{draft?.GetType().Name} is stored";

        // Act
        var result = await controller!.StoreDraft(draft!).ConfigureAwait(false);

        // Assert
        result.Should()
              .BeOfType<OkObjectResult>()
              .Which.StatusCode
              .Should()
              .Be(StatusCodes.Status200OK);
        result.Should()
              .BeOfType<OkObjectResult>()
              .Which.Value.Should().Be(resultValue);
        draftStorageService?.VerifyAll();
    }

    [Test]
    public async Task StoreDraft_WhenModelIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var errorKey = "DraftStorage";
        var errorMessage = "Invalid model state";
        controller?.ModelState.AddModelError(errorKey, errorMessage);
        draftStorageService?.Setup(ds => ds.CreateAsync(key, draft!))
            .Verifiable(Times.Never);

        // Act
        var result = await controller!.StoreDraft(draft!).ConfigureAwait(false);

        // Assert
        result.Should()
              .BeOfType<BadRequestObjectResult>()
              .Which.StatusCode
              .Should()
              .Be(StatusCodes.Status400BadRequest);
        draftStorageService?.VerifyAll();
    }

    [Test]
    public async Task RestoreDraft_WhenDraftExistsInCache_ReturnsDraftAtActionResult()
    {
        // Arrange
        draftStorageService?.Setup(ds => ds.RestoreAsync(key))
            .ReturnsAsync(draft)
            .Verifiable(Times.Once);

        // Act
        var result = await controller!.RestoreDraft().ConfigureAwait(false);

        // Assert
        result.Should()
              .BeOfType<OkObjectResult>()
              .Which.StatusCode
              .Should()
              .Be(StatusCodes.Status200OK);
        result.Should()
             .BeOfType<OkObjectResult>()
             .Which.Value.Should().Be(draft);
        draftStorageService?.VerifyAll();
    }

    [Test]
    public async Task RestoreDraft_WhenDraftIsAbsentInCache_ReturnsDefaultDraftAtActionResult()
    {
        // Arrange
        draftStorageService?.Setup(ds => ds.RestoreAsync(key))
            .ReturnsAsync(default(TeacherWithNameDto))
            .Verifiable(Times.Once);

        // Act
        var result = await controller!.RestoreDraft().ConfigureAwait(false);

        // Assert
        result.Should()
              .BeOfType<OkObjectResult>()
              .Which.StatusCode
              .Should()
              .Be(StatusCodes.Status200OK);
        result.Should()
             .BeOfType<OkObjectResult>()
             .Which.Value.Should().Be(default(TeacherWithNameDto));
        draftStorageService?.VerifyAll();
    }

    private TeacherWithNameDto GetTeacherWithNameDtoDraft()
    {
        var teacherFacker = new Faker<TeacherWithNameDto>()
            .RuleFor(t => t.Name, f => f.Name.LastName());
        return teacherFacker.Generate();
    }
}