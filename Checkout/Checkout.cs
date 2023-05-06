using Checkout.Models;

namespace Checkout;

public class Checkout
{
    public double Total { get; internal set; }

    private IDictionary<Item, ItemPrice> _rules;

    public Checkout(IDictionary<Item, ItemPrice> rules)
    {
        this._rules = rules;
    }

    public void Scan(Item item)
    {
        var itemUnitPrice = _rules[item].UnitPrice;

        Total += itemUnitPrice;
    }
}
