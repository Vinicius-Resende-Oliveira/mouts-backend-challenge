using Ambev.DeveloperEvaluation.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    /// <summary>
    /// Sale number
    /// </summary>

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SaleNumber { get; private set; }

    /// <summary>
    /// Date and time when the sale was made
    /// </summary>
    public DateTime SaleDate { get; private set; }

    /// <summary>
    /// Customer name or identifier
    /// </summary>
    public string Customer { get; private set; } = string.Empty;

    /// <summary>
    /// Total value of the sale
    /// </summary>
    public decimal TotalValue => Items.Sum(i => i.TotalValue);

    /// <summary>
    /// Branch where the sale was made
    /// </summary>
    public string Branch { get; private set; } = string.Empty;

    /// <summary>
    /// Indicates if the sale was cancelled
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// List of items in the sale
    /// </summary>
    public List<SaleItem> Items { get; private set; } = new();

    public Sale() { }

    public Sale(DateTime saleDate, string customer, string branch, List<SaleItem> items)
    {
        SaleDate = saleDate;
        Customer = customer;
        Branch = branch;
        Items = items;
        CreatedAt = DateTime.UtcNow;
        IsCancelled = false;
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}