namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
public class UpdateSaleRequest
{
    public Guid Id { get; set; }
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
    /// List of items in the sale
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
}

public class UpdateSaleItemRequest
{
    /// <summary>
    /// Reference to the product
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Quantity of the product sold
    /// </summary>
    public int Quantity { get; set; }
}