using Ambev.DeveloperEvaluation.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    /// <summary>
    /// Sale number
    /// </summary>

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SaleNumber { get; set; }

    /// <summary>
    /// Date and time when the sale was made
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Customer name or identifier
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// Branch where the sale was made
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the sale was cancelled
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// List of items in the sale
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

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

    public void SetItems(List<SaleItem> items)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items), "Items cannot be null.");
    }
}