namespace Checkout.Tests;

public class CheckoutTests
{
    [Fact]
    public void NewCheckout_ReturnsTotalZero()
    {
        var checkout = new Checkout();

        Assert.Equal(0, checkout.Total);
    }
}