using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.TestData
{
    public static class SaleFakeData
    {
        public static Faker<CreateSaleRequest> GetSaleFaker(Guid? cartId = null)
        {
            return new Faker<CreateSaleRequest>()
                .RuleFor(s => s.CartId, f => cartId ?? Guid.NewGuid())
                .RuleFor(s => s.SaleDate, f => f.Date.Recent())
                .RuleFor(s => s.Customer, f => f.Person.FirstName)
                .RuleFor(s => s.Branch, f => f.Company.CompanyName());
        }

        public static CreateSaleRequest GenerateValidSale(Guid? cartId = null)
        {
            return GetSaleFaker(cartId).Generate();
        }
    }
}
