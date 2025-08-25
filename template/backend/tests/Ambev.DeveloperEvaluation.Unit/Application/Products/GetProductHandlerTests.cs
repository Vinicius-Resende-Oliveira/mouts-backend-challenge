using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
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

public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductQueryHandler> _logger;
    private readonly GetProductQueryHandler _handler;

    public GetProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetProductQueryHandler>>();
        _handler = new GetProductQueryHandler(_productRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Valid ID Given When fetch product Then return product")]
    public async Task Handle_ValidRequest_ReturnsProduct()
    {
        // Arrange
        var command = GetProductHandlerTestData.GenerateValidCommand();
        var product = ProductHandlerTestData.GenerateValidProduct();
        var result = GetProductHandlerTestData.GenerateValidResult(product);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<GetProductResult>(product).Returns(result);

        // Act
        var getProductResult = await _handler.Handle(new GetProductQuery(command.Id), CancellationToken.None);

        // Assert
        getProductResult.Should().NotBeNull();
        getProductResult.Id.Should().Be(product.Id);
        getProductResult.Title.Should().Be(product.Title);
        await _productRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Invalid ID Given When fetching product Then throw validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var invalidCommand = new GetProductQuery(Guid.Empty);

        // Act
        Func<Task> act = () => _handler.Handle(invalidCommand, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given product does not exist When searching for product Then throws not found exception")]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = GetProductHandlerTestData.GenerateValidCommand();
        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Product?)null);

        // Act
        Func<Task> act = () => _handler.Handle(new GetProductQuery(command.Id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Product with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Given existing product When fetch product Then map entity to result")]
    public async Task Handle_ValidRequest_MapsProductToResult()
    {
        // Arrange
        var command = GetProductHandlerTestData.GenerateValidCommand();
        var product = ProductHandlerTestData.GenerateValidProduct(); 
        var result = GetProductHandlerTestData.GenerateValidResult(product);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<GetProductResult>(product).Returns(result);

        // Act
        await _handler.Handle(new GetProductQuery(command.Id), CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<GetProductResult>(product);
    }
}
