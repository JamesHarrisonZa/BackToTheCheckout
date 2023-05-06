namespace Checkout;

public class Checkout
{
    public double Total { get; internal set; }

    private IDictionary<Item, double> _rules;

    public Checkout(IDictionary<Item, double> rules)
    {
        this._rules = rules;
    }

    public void Scan(Item item)
    {
        var itemUnitPrice = _rules[item];

        Total += itemUnitPrice;
    }
}
