using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Products.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class UpdateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateProductHandler>>();
        _handler = new UpdateProductHandler(_productRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid data When update product Then return success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = UpdateProductHandlerTestData.GenerateValidCommand();
        var product = ProductHandlerTestData.GenerateValidProduct();
        var result = UpdateProductHandlerTestData.GenerateValidResult(product.Id, command);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map(command, product);
        _mapper.Map<UpdateProductResult>(product).Returns(result);

        // Act
        var updateProductResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        updateProductResult.Should().NotBeNull();
        updateProductResult.Id.Should().Be(product.Id);
        await _productRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        _productRepository.Received(1).Update(product);
        await _productRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid data When update product Then throw validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = UpdateProductHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given product does not exist When updating product Then throws not found exception")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = UpdateProductHandlerTestData.GenerateValidCommand();
        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Product?)null);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Product with id {command.Id} not found");
    }

    [Fact(DisplayName = "Given valid command When manipulate Then maps command to product entity")]
    public async Task Handle_ValidRequest_MapsCommandToProduct()
    {
        // Arrange
        var command = UpdateProductHandlerTestData.GenerateValidCommand();
        var product = ProductHandlerTestData.GenerateValidProduct();

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(product);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map(command, product);
    }

}
