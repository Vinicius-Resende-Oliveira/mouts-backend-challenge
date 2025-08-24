using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.TestData
{
    public static class CartFakeData
    {
        public static Faker<BaseCartItem> GetCartItemFaker(Guid? productId = null)
        {
            return new Faker<BaseCartItem>()
                .RuleFor(ci => ci.ProductId, f => productId ?? Guid.NewGuid())
                .RuleFor(ci => ci.Quantity, f => f.Random.Int(1, 20));
        }

        public static BaseCartItem GenerateValidCartItem(Guid? productId = null)
        {
            return GetCartItemFaker(productId).Generate();
        }

        public static List<BaseCartItem> GenerateCartItems(int count = 2)
        {
            var items = new List<BaseCartItem>();
            for (int i = 0; i < count; i++)
            {
                items.Add(GenerateValidCartItem());
            }
            return items;
        }
    }
}
