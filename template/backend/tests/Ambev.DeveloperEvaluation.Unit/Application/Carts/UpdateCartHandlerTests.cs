using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

public class UpdateCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly UpdateCartHandler _handler;

    public UpdateCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateCartHandler(_cartRepository, _mapper);
    }

    [Fact(DisplayName = "Dado comando válido Quando atualizar carrinho Então retorna sucesso")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var command = UpdateCartHandlerTestData.GenerateValidCommand();
        var cart = CartHandlerTestData.GenerateValidCart();
        var result = UpdateCartHandlerTestData.GenerateValidResult(cart.Id, command);
        command.Id = cart.Id;

        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map(command, cart);
        _mapper.Map<UpdateCartResult>(cart).Returns(result);

        // Act
        var updateResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        updateResult.Should().NotBeNull();
        updateResult.Id.Should().Be(cart.Id);
        _cartRepository.Received(1).Update(cart);
        await _cartRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Dado comando inválido Quando atualizar carrinho Então lança exceção de validação")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = UpdateCartHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Dado carrinho inexistente Quando atualizar carrinho Então lança exceção de não encontrado")]
    public async Task Handle_CartNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = UpdateCartHandlerTestData.GenerateValidCommand();
        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Cart?)null);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Cart with id {command.Id} not found");
    }

    [Fact(DisplayName = "Dado comando válido Quando manipular Então mapeia comando para entidade carrinho")]
    public async Task Handle_ValidRequest_MapsCommandToCart()
    {
        // Arrange
        var command = UpdateCartHandlerTestData.GenerateValidCommand();
        var cart = CartHandlerTestData.GenerateValidCart();

        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(cart);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map(command, cart);
    }

    [Fact(DisplayName = "Dado produtos duplicados Quando atualizar carrinho Então lança exceção de validação")]
    public async Task Handle_DuplicateProducts_ThrowsValidationException()
    {
        // Arrange
        var command = UpdateCartHandlerTestData.GenerateValidCommand();
        if (command.Products == null || command.Products.Count == 0)
        {
            command.Products = new List<BaseCartItem>
            {
                Substitute.For<BaseCartItem>(),
                Substitute.For<BaseCartItem>()
            };
        }
        // Força dois produtos com o mesmo ProductId
        var productId = Guid.NewGuid();
        command.Products[0].ProductId = productId;
        command.Products[1].ProductId = productId;

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*same product id*");
    }
}
