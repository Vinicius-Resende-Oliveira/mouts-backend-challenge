using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

public class CreateCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly CreateCartHandler _handler;

    public CreateCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateCartHandler(_cartRepository, _mapper);
    }

    [Fact(DisplayName = "Given a valid command When creating cart Then returns success")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        var cart = CartHandlerTestData.GenerateValidCart();
        var result = CreateCartHandlerTestData.GenerateValidResult();

        _mapper.Map<Cart>(command).Returns(cart);
        _cartRepository.CreateAsync(cart, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<CreateCartResult>(cart).Returns(result);

        // Act
        var createResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        createResult.Should().NotBeNull();
        createResult.Products.Should().NotBeNull();
        await _cartRepository.Received(1).CreateAsync(cart, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an invalid command When creating cart Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = CreateCartHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given a valid command When creating cart Then maps command to entity")]
    public async Task Handle_ValidRequest_MapsCommandToCart()
    {
        // Arrange
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        var cart = CartHandlerTestData.GenerateValidCart();

        _mapper.Map<Cart>(command).Returns(cart);
        _cartRepository.CreateAsync(cart, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<CreateCartResult>(cart).Returns(CreateCartHandlerTestData.GenerateValidResult());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<Cart>(command);
    }
}
