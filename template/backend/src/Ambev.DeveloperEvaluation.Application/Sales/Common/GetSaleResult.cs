namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Common response  model for sale operations
/// </summary>
public class GetSaleResult
{
    /// <summary>
    /// The unique identifier of the sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Sale number
    /// </summary>
    public int SaleNumber { get; private set; }

    /// <summary>
    /// Date and time when the sale was made
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Customer name or identifier
    /// </summary>
    public required string Customer { get; set; }

    /// <summary>
    /// Branch where the sale was made
    /// </summary>
    public required string Branch { get; set; }

    /// <summary>
    /// Indicates if the sale was cancelled
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Total value of the sale
    /// </summary>
    public decimal TotalValue => Items.Sum(i => i.TotalValue);

    /// <summary>
    /// List of items in the sale
    /// </summary>
    public List<GetSaleItemResult> Items { get; set; } = new();
}

public class GetSaleItemResult
{
    /// <summary>
    /// Reference to the product
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Quantity of the product sold
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// Unit price of the product
    /// </summary>
    public decimal UnitPrice { get; set; }
    /// <summary>
    /// Total value for this item (Quantity * UnitPrice)
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Discount percentage applied to this item (0.10 = 10%)
    /// </summary>
    public decimal Discount { get; private set; }
}