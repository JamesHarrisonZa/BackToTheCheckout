namespace Checkout.Models;

public class Rule
{
    public string Item { get; }
    public double UnitPrice { get; }

    public Rule(string item, int unitPrice)
    {
        Item = item;
        UnitPrice = unitPrice;
    }
}
