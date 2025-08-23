namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleRequest
    {
        /// <summary>
        /// Reference to the cart
        /// </summary>
        public Guid CartId { get; set; }

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
    }
}
