using AutoMapper;
using College.BLL.Interfaces;
using College.DAL.Repositories.Interfaces.Base;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace College.XUnitTest.MediatRTests.Teacher;

//public class CreateTeacherHandlerTests
//{
//    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
//    private readonly Mock<IMapper> _mockMapper;
//    private readonly Mock<ILoggerService> _mockLogger;

//    public CreateTeacherHandlerTests()
//    {
//        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
//        _mockMapper = new Mock<IMapper>();
//        _mockLogger = new Mock<ILoggerService>();
//    }

//    [Fact]
//    public async Task Handle_ValidCreateFactCommand_WithExistingImageAndStreetcode_ShouldSucceed()
//    {
//        // Arrange
//        var request = GetValidCreateFactRequest();
//        SetupMock(request);
//        var handler = CreateHandler();
//        var command = new CreateFactCommand(request);
//        var cancellationToken = CancellationToken.None;

//        // Act
//        var result = await handler.Handle(command, cancellationToken);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//    }

//    [Fact]
//    public async Task Handle_InvalidCreateFactCommand_WithNonexistentImage_ShouldReturnSingleError()
//    {
//        // Arrange
//        var request = GetImageNotExistCreateFactRequest();
//        var expectedErrorMessage = string.Format(
//            ErrorMessages.EntityByIdNotFound,
//            nameof(Image),
//            -1);
//        SetupMock(request);
//        var handler = CreateHandler();
//        var command = new CreateFactCommand(request);
//        var cancellationToken = CancellationToken.None;

//        // Act
//        var result = await handler.Handle(command, cancellationToken);

//        // Assert
//        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
//    }

//    [Fact]
//    public async Task Handle_InvalidCreateFactCommand_WithNonexistentStreetcode_ShouldReturnSingleError()
//    {
//        // Arrange
//        var request = GetStreetcodeNotExistCreateFactRequest();
//        string expectedErrorMessage = string.Format(
//            ErrorMessages.EntityByIdNotFound,
//            nameof(StreetcodeContent),
//            request.StreetcodeId);
//        SetupMock(request);
//        var handler = CreateHandler();
//        var command = new CreateFactCommand(request);
//        var cancellationToken = CancellationToken.None;

//        // Act
//        var result = await handler.Handle(command, cancellationToken);

//        // Assert
//        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
//    }

//    [Fact]
//    public async Task Handle_AnyCreateFactCommand_WithAnyImageAndAnyStreetcode_ShouldReturnResultOfCorrectType()
//    {
//        // Arrange
//        var request = GetValidCreateFactRequest();
//        var expectedType = typeof(Result<CreateFactDto>);
//        SetupMock(request);
//        var handler = CreateHandler();
//        var command = new CreateFactCommand(request);
//        var cancellationToken = CancellationToken.None;

//        // Act
//        var result = await handler.Handle(command, cancellationToken);

//        // Assert
//        result.Should().BeOfType(expectedType);
//    }

//    private CreateFactHandler CreateHandler()
//    {
//        return new CreateFactHandler(
//            repository: _mockRepositoryWrapper.Object,
//            mapper: _mockMapper.Object,
//            logger: _mockLogger.Object);
//    }

//    private void SetupMock(CreateFactDto request)
//    {
//        var image = request.ImageId switch
//        {
//            1 => new Image { Id = request.ImageId },
//            _ => null,
//        };

//        var streetcode = request.StreetcodeId switch
//        {
//            1 => new StreetcodeContent { Id = request.StreetcodeId },
//            _ => null,
//        };

//        var factEntity = new Fact
//        {
//            Id = 1,
//        };

//        _mockRepositoryWrapper
//            .Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
//                AnyEntityPredicate<Image>(),
//                AnyEntityInclude<Image>()))
//            .ReturnsAsync(image);

//        _mockRepositoryWrapper
//            .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
//                AnyEntityPredicate<StreetcodeContent>(),
//                AnyEntityInclude<StreetcodeContent>()))
//            .ReturnsAsync(streetcode);

//        _mockRepositoryWrapper
//            .Setup(r => r.FactRepository.Create(It.IsAny<Fact>()))
//            .Returns(factEntity);

//        _mockRepositoryWrapper
//            .Setup(r => r.SaveChangesAsync())
//            .ReturnsAsync(1);

//        _mockMapper
//            .Setup(m => m.Map<Fact>(It.Is<object>(obj => obj is CreateFactDto)))
//            .Returns(factEntity);
//    }

//    private static CreateFactDto GetValidCreateFactRequest()
//    {
//        return new CreateFactDto(
//            Title: "Title1",
//            FactContent: "Fact content 1",
//            ImageId: 1,
//            StreetcodeId: 1);
//    }

//    private static CreateFactDto GetImageNotExistCreateFactRequest()
//    {
//        return new CreateFactDto(
//            Title: "Title2",
//            FactContent: "Fact content 2",
//            ImageId: -1,
//            StreetcodeId: 1);
//    }

//    private static CreateFactDto GetStreetcodeNotExistCreateFactRequest()
//    {
//        return new CreateFactDto(
//            Title: "Title3",
//            FactContent: "Fact content 3",
//            ImageId: 1,
//            StreetcodeId: -1);
//    }

//    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
//    {
//        return It.IsAny<Expression<Func<TEntity, bool>>>();
//    }

//    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
//    {
//        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
//    }
//}
