using Checkout.Models;

namespace Checkout;

public class Checkout
{
    public double Total { get; internal set; }

    private IDictionary<Item, ItemPrice> _rules;

    private IDictionary<Item, int> _scannedItems;

    public Checkout(IDictionary<Item, ItemPrice> rules)
    {
        _rules = rules;
        _scannedItems = new Dictionary<Item, int>();
    }

    public void Scan(Item item)
    {
        if (!_scannedItems.ContainsKey(item))
            _scannedItems.Add(item, 0);
        _scannedItems[item]++;

        //HasSpecial && TriggersSpecial
        if (_rules[item]?.Special != null && _scannedItems[item] % _rules[item]?.Special.Quantity == 0)
        {
            var specialPrice = _rules[item].Special.Price;
            var discountedPrice = -1 * (_rules[item].UnitPrice * (_rules[item].Special.Quantity - 1)) + specialPrice;

            Total += discountedPrice;
        }
        else
        {
            var itemUnitPrice = _rules[item].UnitPrice;
            Total += itemUnitPrice;
        }
    }
}
