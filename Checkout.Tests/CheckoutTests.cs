using Checkout.Models;

namespace Checkout.Tests;

public class CheckoutTests
{
    [Fact]
    public void NewCheckout_ReturnsTotalZero()
    {
        var rules = new Dictionary<Item, ItemPrice>() { };
        var checkout = new Checkout(rules);

        Assert.Equal(0, checkout.Total);
    }

    [Theory]
    [InlineData(new[] { Item.A }, 50)]
    [InlineData(new[] { Item.A, Item.B }, 80)]
    [InlineData(new[] { Item.A, Item.B, Item.C }, 100)]
    [InlineData(new[] { Item.A, Item.B, Item.C, Item.D }, 115)]
    public void Scan_NoSpecials_ReturnsSumOfUnitPrices(Item[] goods, double expectedTotal)
    {
        var rules = new Dictionary<Item, ItemPrice>() {
            { Item.A, new ItemPrice(50) },
            { Item.B, new ItemPrice(30) },
            { Item.C, new ItemPrice(20) },
            { Item.D, new ItemPrice(15) },
        };

        var checkout = new Checkout(rules);

        foreach (var item in goods)
        {
            checkout.Scan(item);
        }

        Assert.Equal(expectedTotal, checkout.Total);
    }

    [Theory]
    [InlineData(new[] { Item.A, Item.A }, 100)]
    //[InlineData(new[] { Item.A, Item.A, Item.A }, 130)]
    //[InlineData(new[] { Item.A, Item.A, Item.A, Item.A }, 180)]
    public void Scan_WithSpecials_AppliesDiscount(Item[] goods, double expectedTotal)
    {
        var rules = new Dictionary<Item, ItemPrice>() {
            { Item.A, new ItemPrice(50) },
        };

        var checkout = new Checkout(rules);

        foreach (var item in goods)
        {
            checkout.Scan(item);
        }

        Assert.Equal(expectedTotal, checkout.Total);
    }
}