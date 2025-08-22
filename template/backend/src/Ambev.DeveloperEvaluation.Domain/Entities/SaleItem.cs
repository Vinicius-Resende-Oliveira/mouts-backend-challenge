using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    /// <summary>
    /// Reference to the sale
    /// </summary>
    public Guid SaleId { get; private set; }
    public Sale? Sale { get; private set; }

    /// <summary>
    /// Reference to the product
    /// </summary>
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }

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

    public SaleItem() { }

    public SaleItem(Guid saleId, Guid productId, int quantity, decimal unitPrice)
    {
        SaleId = saleId;
        ProductId = productId;
        SetQuantityAndPrice(quantity, unitPrice);
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets quantity and unit price, then calculates discount and total value.
    /// </summary>
    public void SetQuantityAndPrice(int quantity, decimal unitPrice)
    {
        if (quantity > 20)
            throw new InvalidOperationException("Cannot sell more than 20 identical items.");
        if (quantity < 1)
            throw new InvalidOperationException("Quantity must be at least 1.");

        Quantity = quantity;
        UnitPrice = unitPrice;
        CalculateDiscountAndTotal();
    }

    /// <summary>
    /// Calculates discount and total value according to business rules.
    /// </summary>
    public void CalculateDiscountAndTotal()
    {
        Discount = Quantity switch
        {
            >= 10 and <= 20 => 0.20m,
            >= 4 and < 10 => 0.10m,
            < 4 => 0.00m,
            _ => throw new InvalidOperationException("Invalid quantity for discount calculation.")
        };

        var gross = UnitPrice * Quantity;
        var discountValue = gross * Discount;
        TotalValue = gross - discountValue;
    }
}
