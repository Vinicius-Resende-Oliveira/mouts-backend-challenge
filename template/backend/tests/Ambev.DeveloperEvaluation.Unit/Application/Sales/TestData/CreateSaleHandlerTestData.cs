using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using NSubstitute;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// </summary>
    private static readonly Faker<CreateSaleCommand> createSaleCommandHandlerFaker = new Faker<CreateSaleCommand>()
        .RuleFor(u => u.CartId, f => Guid.NewGuid())
        .RuleFor(u => u.SaleDate, f => DateTime.UtcNow)
        .RuleFor(u => u.Customer, f => f.Internet.UserName())
        .RuleFor(u => u.Branch, f => f.Commerce.Department());

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return createSaleCommandHandlerFaker.Generate();
    }

    public static CreateSaleCommand GenerateInvalidCommand()
    {
        var command = Substitute.ForPartsOf<CreateSaleCommand>();
        command.Customer = String.Empty;
        command.Branch = String.Empty;
        command.CartId = Guid.Empty;
        return command;
    }

    public static CreateSaleResult GenerateValidResult(Sale sale)
    {
        return new CreateSaleResult() {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            Customer = sale.Customer,
            Branch = sale.Branch,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items.Select(i => new GetSaleItemResult
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalValue = i.TotalValue
            }).ToList(),
        };
    }
}
