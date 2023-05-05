namespace Checkout.Tests;

public class CheckoutTests
{
    [Fact]
    public void NewCheckout_ReturnsTotalZero()
    {
        var checkout = new Checkout(new List<Rule>() { });

        Assert.Equal(0, checkout.Total);
    }

    [Fact]
    public void Scan_OneItem_ReturnsUnitPrice()
    {
        var rule = new Rule("A", 50);
        var rules = new List<Rule>() { rule };

        var checkout = new Checkout(rules);
        checkout.Scan("A");

        Assert.Equal(50, checkout.Total);
    }
}