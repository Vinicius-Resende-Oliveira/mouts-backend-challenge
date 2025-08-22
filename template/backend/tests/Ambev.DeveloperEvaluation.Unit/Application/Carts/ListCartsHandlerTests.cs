using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

public class ListCartsHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ListCartsHandler _handler;

    public ListCartsHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListCartsHandler(_cartRepository, _mapper);
    }

    [Fact(DisplayName = "Dado filtros e paginação Quando listar carrinhos Então retorna lista paginada")]
    public async Task Handle_ValidRequest_ReturnsPaginatedList()
    {
        // Arrange
        var carts = CartHandlerTestData.GenerateListCarts(5);
        var results = carts.Select(GetCartHandlerTestData.GenerateValidResult).ToList();
        var queryable = carts.AsQueryable();

        var command = new ListCartsCommand
        {
            Page = 1,
            Size = 5,
            UserId = null,
            MinDate = null,
            MaxDate = null,
            Order = null
        };

        _cartRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _cartRepository.Filter(Arg.Any<IQueryable<Cart>>(), Arg.Any<string>(), Arg.Any<string?>()).Returns(queryable);
        _cartRepository.FilterRange(Arg.Any<IQueryable<Cart>>(), Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<object?>()).Returns(queryable);

        _mapper.ProjectTo<GetCartResult>(Arg.Any<IQueryable<Cart>>()).Returns(results.AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        paginatedList.Should().NotBeNull();
        paginatedList.Should().HaveCount(5);
        paginatedList.Select(c => c.Id).Should().BeEquivalentTo(results.Select(r => r.Id));
    }

    [Fact(DisplayName = "Dado filtros Quando listar carrinhos Então aplica filtros corretamente")]
    public async Task Handle_WithFilters_AppliesFilters()
    {
        // Arrange
        var carts = CartHandlerTestData.GenerateListCarts(3);
        var queryable = carts.AsQueryable();

        var userId = Guid.NewGuid();
        var minDate = DateTime.UtcNow.AddDays(-10);
        var maxDate = DateTime.UtcNow.AddDays(10);

        var command = new ListCartsCommand
        {
            Page = 1,
            Size = 3,
            UserId = userId,
            MinDate = minDate,
            MaxDate = maxDate,
            Order = "Date desc"
        };

        _cartRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _cartRepository.Filter(Arg.Any<IQueryable<Cart>>(), Arg.Any<string>(), Arg.Any<string?>()).Returns(callInfo => callInfo.Arg<IQueryable<Cart>>());
        _cartRepository.FilterRange(Arg.Any<IQueryable<Cart>>(), Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<object?>()).Returns(callInfo => callInfo.Arg<IQueryable<Cart>>());
        _cartRepository.OrderByFields(Arg.Any<IQueryable<Cart>>(), Arg.Any<(string, bool)[]>()).Returns(callInfo => callInfo.Arg<IQueryable<Cart>>());

        _mapper.ProjectTo<GetCartResult>(Arg.Any<IQueryable<Cart>>()).Returns(carts.Select(GetCartHandlerTestData.GenerateValidResult).AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _cartRepository.Received(1).Filter(Arg.Any<IQueryable<Cart>>(), nameof(GetCartResult.UserId), userId.ToString());
        _cartRepository.Received(1).FilterRange(Arg.Any<IQueryable<Cart>>(), nameof(GetCartResult.Date), minDate, maxDate);
        _cartRepository.Received(1).OrderByFields(Arg.Any<IQueryable<Cart>>(), Arg.Any<(string, bool)[]>());
        paginatedList.Should().NotBeNull();
    }

    [Fact(DisplayName = "Dado lista vazia Quando listar carrinhos Então retorna lista vazia")]
    public async Task Handle_EmptyList_ReturnsEmptyPaginatedList()
    {
        // Arrange
        var carts = new List<Cart>();
        var queryable = carts.AsQueryable();

        var command = new ListCartsCommand
        {
            Page = 1,
            Size = 10
        };

        _cartRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _mapper.ProjectTo<GetCartResult>(Arg.Any<IQueryable<Cart>>()).Returns(new List<GetCartResult>().AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        paginatedList.Should().NotBeNull();
        paginatedList.Should().BeEmpty();
    }
}
