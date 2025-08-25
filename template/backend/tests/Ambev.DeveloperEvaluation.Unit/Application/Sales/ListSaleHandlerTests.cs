using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListSalesQueryHandler> _logger;
    private readonly ListSalesQueryHandler _handler;

    public ListSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ListSalesQueryHandler>>();
        _handler = new ListSalesQueryHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given filters and pagination When listing sales Then returns paginated list")]
    public async Task Handle_ValidRequest_ReturnsPaginatedList()
    {
        // Arrange
        var sales = SaleHandlerTestData.GenerateListSales(5);
        var results = sales.Select(GetSaleHandlerTestData.GenerateValidResult).ToList();
        var queryable = sales.AsQueryable();

        var command = new ListSalesQuery
        {
            Page = 1,
            Size = 5,
            Customer = null,
            Branch = null,
            SaleNumber = null,
            IsCancelled = null,
            MinSaleDate = null,
            MaxSaleDate = null,
            Order = null
        };

        _saleRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _saleRepository.Filter(Arg.Any<IQueryable<Sale>>(), Arg.Any<string>(), Arg.Any<string?>()).Returns(queryable);
        _saleRepository.FilterRange(Arg.Any<IQueryable<Sale>>(), Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<object?>()).Returns(queryable);
        _saleRepository.BoolFilter(Arg.Any<IQueryable<Sale>>(), Arg.Any<string>(), Arg.Any<bool?>()).Returns(queryable);

        _mapper.ProjectTo<GetSaleResult>(Arg.Any<IQueryable<Sale>>()).Returns(results.AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        paginatedList.Should().NotBeNull();
        paginatedList.Should().HaveCount(5);
        paginatedList.Select(s => s.Id).Should().BeEquivalentTo(results.Select(r => r.Id));
    }

    [Fact(DisplayName = "Given filters When listing sales Then applies filters correctly")]
    public async Task Handle_WithFilters_AppliesFilters()
    {
        // Arrange
        var sales = SaleHandlerTestData.GenerateListSales(3);
        var queryable = sales.AsQueryable();

        var customer = "Test Customer";
        var branch = "Branch 1";
        var saleNumber = 123;
        var isCancelled = false;
        var minDate = DateTime.UtcNow.AddDays(-10);
        var maxDate = DateTime.UtcNow.AddDays(10);

        var command = new ListSalesQuery
        {
            Page = 1,
            Size = 3,
            Customer = customer,
            Branch = branch,
            SaleNumber = saleNumber,
            IsCancelled = isCancelled,
            MinSaleDate = minDate,
            MaxSaleDate = maxDate,
            Order = "SaleDate desc"
        };

        _saleRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _saleRepository.Filter(Arg.Any<IQueryable<Sale>>(), Arg.Any<string>(), Arg.Any<string?>()).Returns(callInfo => callInfo.Arg<IQueryable<Sale>>());
        _saleRepository.FilterRange(Arg.Any<IQueryable<Sale>>(), Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<object?>()).Returns(callInfo => callInfo.Arg<IQueryable<Sale>>());
        _saleRepository.BoolFilter(Arg.Any<IQueryable<Sale>>(), Arg.Any<string>(), Arg.Any<bool?>()).Returns(callInfo => callInfo.Arg<IQueryable<Sale>>());
        _saleRepository.OrderByFields(Arg.Any<IQueryable<Sale>>(), Arg.Any<(string, bool)[]>()).Returns(callInfo => callInfo.Arg<IQueryable<Sale>>());

        _mapper.ProjectTo<GetSaleResult>(Arg.Any<IQueryable<Sale>>()).Returns(sales.Select(GetSaleHandlerTestData.GenerateValidResult).AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _saleRepository.Received(1).Filter(Arg.Any<IQueryable<Sale>>(), nameof(GetSaleResult.Customer), customer);
        _saleRepository.Received(1).Filter(Arg.Any<IQueryable<Sale>>(), nameof(GetSaleResult.Branch), branch);
        _saleRepository.Received(1).Filter(Arg.Any<IQueryable<Sale>>(), nameof(GetSaleResult.SaleNumber), saleNumber.ToString());
        _saleRepository.Received(1).BoolFilter(Arg.Any<IQueryable<Sale>>(), nameof(GetSaleResult.IsCancelled), isCancelled);
        _saleRepository.Received(1).FilterRange(Arg.Any<IQueryable<Sale>>(), nameof(GetSaleResult.SaleDate), minDate, maxDate);
        _saleRepository.Received(1).OrderByFields(Arg.Any<IQueryable<Sale>>(), Arg.Any<(string, bool)[]>());
        paginatedList.Should().NotBeNull();
    }

    [Fact(DisplayName = "Given empty list When listing sales Then returns empty paginated list")]
    public async Task Handle_EmptyList_ReturnsEmptyPaginatedList()
    {
        // Arrange
        var sales = new List<Sale>();
        var queryable = sales.AsQueryable();

        var command = new ListSalesQuery
        {
            Page = 1,
            Size = 10
        };

        _saleRepository.GetAll(Arg.Any<CancellationToken>()).Returns(queryable);
        _mapper.ProjectTo<GetSaleResult>(Arg.Any<IQueryable<Sale>>()).Returns(new List<GetSaleResult>().AsQueryable());

        // Act
        var paginatedList = await _handler.Handle(command, CancellationToken.None);

        // Assert
        paginatedList.Should().NotBeNull();
        paginatedList.Should().BeEmpty();
    }
}