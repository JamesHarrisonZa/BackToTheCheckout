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
    [InlineData(new[] { Item.A, Item.A, Item.A }, 130)] //EligibleForSpecial A x1
    [InlineData(new[] { Item.A, Item.A, Item.A, Item.A }, 180)]
    [InlineData(new[] { Item.A, Item.A, Item.A, Item.A, Item.A, Item.A }, 260)] //EligibleForSpecial A x2

    [InlineData(new[] { Item.A, Item.A, Item.A, Item.B }, 160)]
    [InlineData(new[] { Item.A, Item.A, Item.A, Item.B, Item.B }, 175)] //EligibleForSpecial A & B x1
    [InlineData(new[] { Item.A, Item.A, Item.A, Item.B, Item.B, Item.C, Item.D }, 210)]
    public void Scan_WithSpecials_AppliesDiscounts(Item[] goods, double expectedTotal)
    {
        var rules = new Dictionary<Item, ItemPrice>() {
            { Item.A, new ItemPrice(50, new Special(3, 130)) },
            { Item.B, new ItemPrice(30, new Special(2, 45)) },
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

    // When I got to this point I found I can use the same rule structure to cover the 2 for 1 scenario

    [Theory]
    [InlineData(new[] { Item.A, Item.A, Item.A }, 100)]
    [InlineData(new[] { Item.B, Item.B, Item.B }, 60)]
    [InlineData(new[] { Item.C, Item.C, Item.C }, 40)]
    [InlineData(new[] { Item.D, Item.D, Item.D }, 30)]
    public void Scan_WithTwoForOneDeal_DoesNotChargeThirdItem(Item[] goods, double expectedTotal)
    {
        var rules = new Dictionary<Item, ItemPrice>() {
            { Item.A, new ItemPrice(50, new Special(3, 100)) },
            { Item.B, new ItemPrice(30, new Special(3, 60)) },
            { Item.C, new ItemPrice(20, new Special(3, 40)) },
            { Item.D, new ItemPrice(15, new Special(3, 30)) },
        };

        var checkout = new Checkout(rules);

        foreach (var item in goods)
        {
            checkout.Scan(item);
        }

        Assert.Equal(expectedTotal, checkout.Total);
    }

    // Scenario "1.99 per pound"
    [Fact]
    public void Scan_WithPricePerPound_AddsRelativePrice()
    {

        var rules = new Dictionary<Item, ItemPrice>() {
            { Item.A, new ItemPrice(1.99) }, //per pound is my unit for this item
        };

        var checkout = new Checkout(rules);
        
        checkout.Scan(Item.A, 1);

        var expectedTotal = 1.99d;
        Assert.Equal(expectedTotal, checkout.Total);
    }
}