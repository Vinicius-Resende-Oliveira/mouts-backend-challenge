namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;


public class UpdateSaleResponse
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
    public decimal TotalValue { get; set; }

    /// <summary>
    /// List of items in the sale
    /// </summary>
    public List<UpdateSaleItemResponse> Items { get; set; } = new();
}

public class UpdateSaleItemResponse
{
    /// <summary>
    /// Reference to the sale
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Quantity sold
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Unit price at the time of sale
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Discount percentage applied to this item (0.10 = 10%)
    /// </summary>
    public decimal Discount { get; private set; }

    /// <summary>
    /// Total value for this item (after discount)
    /// </summary>
    public decimal TotalValue { get; private set; }
}
