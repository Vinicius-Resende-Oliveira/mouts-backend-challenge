using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

public class GetCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly GetCartHandler _handler;

    public GetCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetCartHandler(_cartRepository, _mapper);
    }

    [Fact(DisplayName = "Given a valid command When getting cart Then returns cart result")]
    public async Task Handle_ValidRequest_ReturnsCartResult()
    {
        // Arrange
        var cart = CartHandlerTestData.GenerateValidCart();
        var command = new GetCartCommand(cart.Id);
        var result = GetCartHandlerTestData.GenerateValidResult(cart);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<GetCartResult>(cart).Returns(result);

        // Act
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        getResult.Should().NotBeNull();
        getResult.Id.Should().Be(cart.Id);
        getResult.UserId.Should().Be(cart.UserId);
        getResult.Products.Should().NotBeNull();
        _cartRepository.Received(1).GetByIdAsync(cart.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an invalid command When getting cart Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = GetCartHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given a non-existent cart When getting cart Then throws not found exception")]
    public async Task Handle_CartNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = GetCartHandlerTestData.GenerateValidCommand();
        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Cart?)null);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Cart with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Given a valid cart When getting cart Then maps entity to result")]
    public async Task Handle_ValidRequest_MapsEntityToResult()
    {
        // Arrange
        var cart = CartHandlerTestData.GenerateValidCart();
        var command = new GetCartCommand(cart.Id);
        var result = GetCartHandlerTestData.GenerateValidResult(cart);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<GetCartResult>(cart).Returns(result);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<GetCartResult>(cart);
    }
}
