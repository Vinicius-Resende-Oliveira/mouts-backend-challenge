using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

public class DeleteCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly DeleteCartHandler _handler;
    private readonly ILogger<DeleteCartHandler> _logger;

    public DeleteCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _logger = Substitute.For<ILogger<DeleteCartHandler>>();
        _handler = new DeleteCartHandler(_cartRepository, _logger);
    }

    [Fact(DisplayName = "Given a valid command When deleting cart Then returns success")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var command = DeleteCartHandlerTestData.GenerateValidCommand();
        _cartRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        await _cartRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an invalid command When deleting cart Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = DeleteCartHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given a non-existent cart When deleting cart Then throws not found exception")]
    public async Task Handle_CartNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = DeleteCartHandlerTestData.GenerateValidCommand();
        _cartRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Cart with ID {command.Id} not found");
    }
}
