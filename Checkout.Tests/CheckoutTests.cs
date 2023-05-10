using Checkout.Models;
using Checkout.Exceptions;

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

    // Scenario: no specials. Just unit prices.
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

    // Scenario: "Three for a dollar" type specials.
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

    // Scenario: "2 for 1".
    // NOTE for reviewers: I found I could use the same rule structure
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

    // Scenario: "1.99 per pound".
    // NOTE for reviewers: I needed to introduce scanning/tracking item weights. Polymorphism to the rescue.
    [Theory]
    [InlineData(Item.A, 1, 1.99)]
    [InlineData(Item.A, 1.5, 2.98)] //Rounded to 2 decimal points
    [InlineData(Item.A, 2, 3.98)]
    public void Scan_WithPricePerPound_AddsRelativePrice(Item item, double weight, double expectedTotal)
    {
        var rules = new Dictionary<Item, ItemPrice>() {
            { Item.A, new ItemPrice(1.99) }, //per pound is my unit for this item
        };
        var checkout = new Checkout(rules);

        checkout.Scan(item, weight);
        
        Assert.Equal(expectedTotal, checkout.Total);
    }

    // Scenario: price per pound with special".
    // NOTE for reviewers: I needed to introduce scanning/tracking item weights. Polymorphism to the rescue.
    [Theory]
    [InlineData(Item.A, 1, 1.99)] //Just under special
    [InlineData(Item.A, 3, 4.99)] //Matches special x1
    [InlineData(Item.A, 5, 8.97)] //Special + remaining
    [InlineData(Item.A, 6, 9.98)] //Matches special x2
    public void Scan_WithPricePerPoundSpecial_AddsSpecialPrice(Item item, double weight, double expectedTotal)
    {
        var rules = new Dictionary<Item, ItemPrice>() {
            { Item.A, new ItemPrice(1.99, new Special(3d, 4.99)) },
        };
        var checkout = new Checkout(rules);

        checkout.Scan(item, weight);

        Assert.Equal(expectedTotal, checkout.Total);
    }

    // Scenario: Scanning an item that we don't have a rule for
    [Fact]
    public void Scan_WithUnexpectedItem_ThrowsUnexpectedItemException()
    {
        var unexpectedItem = Item.A;
        var rules = new Dictionary<Item, ItemPrice>() {};
        var checkout = new Checkout(rules);

        var exception = Assert.Throws<UnexpectedItemException>(() => checkout.Scan(unexpectedItem));

        var expectedMessage = $"Unexpected Item: {unexpectedItem}. Missing from pricing rules.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    // Scenario: Scanning an item with weight that we don't have a rule for
    [Fact]
    public void Scan_WithUnexpectedItemWithWeight_ThrowsUnexpectedItemException()
    {
        var unexpectedItem = Item.A;
        var itemWeight = 42;
        var rules = new Dictionary<Item, ItemPrice>() { };
        var checkout = new Checkout(rules);

        var exception = Assert.Throws<UnexpectedItemException>(() => checkout.Scan(unexpectedItem, itemWeight));

        var expectedMessage = $"Unexpected Item: {unexpectedItem}. Missing from pricing rules.";
        Assert.Equal(expectedMessage, exception.Message);
    }
}