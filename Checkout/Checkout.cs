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
        AddToScannedItems(item);

        if (HasSpecial(item) && EligibleForSpecial(item))
            Total += GetDiscountedPrice(item);
        else
            Total += GetItemUnitPrice(item);
    }

    private void AddToScannedItems(Item item)
    {
        if (!_scannedItems.ContainsKey(item))
            _scannedItems.Add(item, 0);

        _scannedItems[item]++;
    }

    private double GetItemUnitPrice(Item item)
    {
        return _rules[item].UnitPrice;
    }

    private bool HasSpecial(Item item)
    {
        return _rules[item]?.Special != null;
    }

    private bool EligibleForSpecial(Item item)
    {
        return _scannedItems[item] % _rules[item]?.Special.Quantity == 0;
    }

    private double GetDiscountedPrice(Item item)
    {
        var specialPrice = GetItemsSpecialPrice(item);

        return -1 * GetPriceAccountedForInTotal(item) + specialPrice;
    }

    private double GetItemsSpecialPrice(Item item)
    {
        return _rules[item].Special.Price;
    }

    private double GetPriceAccountedForInTotal(Item item)
    {
        return (_rules[item].UnitPrice * (_rules[item].Special.Quantity - 1));
    }
}
