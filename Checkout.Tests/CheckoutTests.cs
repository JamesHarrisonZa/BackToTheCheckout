namespace Checkout.Tests;

public class CheckoutTests
{
    [Fact]
    public void NewCheckout_ReturnsTotalZero()
    {
        var rules = new Dictionary<string, double>() {};
        var checkout = new Checkout(rules);

        Assert.Equal(0, checkout.Total);
    }

    [Fact]
    public void Scan_OneItem_ReturnsUnitPrice()
    {
        var rules = new Dictionary<string, double>() { { "A", 50 } };

        var checkout = new Checkout(rules);
        checkout.Scan("A");

        Assert.Equal(50, checkout.Total);
    }

    [Fact]
    public void Scan_TwoSameItem_ReturnsTwiceUnitPrice()
    {
        var rules = new Dictionary<string, double>() { { "A", 50 } };

        var checkout = new Checkout(rules);
        checkout.Scan("A");
        checkout.Scan("A");

        Assert.Equal(100, checkout.Total);
	}

	[Fact]
	public void Scan_TwoItems_ReturnsSumOfUnitPrices()
	{
		var rules = new Dictionary<string, double>() { 
            { "A", 50 }, 
            { "B", 25 }, 
        };

		var checkout = new Checkout(rules);
		checkout.Scan("A");
		checkout.Scan("B");

		Assert.Equal(75, checkout.Total);
	}
}