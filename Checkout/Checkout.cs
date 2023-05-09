using Checkout.Models;

namespace Checkout;

public class Checkout
{
    public double Total { get; private set; }

    private readonly IDictionary<Item, ItemPrice> _rules;

    private readonly IDictionary<Item, int> _scannedItemsCount;

    private const int RoundingPrecision = 2;

    public Checkout(IDictionary<Item, ItemPrice> rules)
    {
        _rules = rules;
        _scannedItemsCount = new Dictionary<Item, int>();
    }

    public void Scan(Item item)
    {
        AddToScannedItems(item);

        if (EligibleForDiscount(item))
            Total += GetDiscountedPrice(item);
        else
            Total += GetItemUnitPrice(item);
    }

    public void Scan(Item item, double weight)
    {
        Total += GetPriceForWeight(item, weight);
    }

    private double GetPriceForWeight(Item item, double weight)
    {
        var itemPrice = weight * GetItemUnitPrice(item);
        var itemPriceRounded = Math.Round(itemPrice, RoundingPrecision);
        return itemPriceRounded;
    }

    private void AddToScannedItems(Item item)
    {
        if (!_scannedItemsCount.ContainsKey(item))
            _scannedItemsCount.Add(item, 0);

        _scannedItemsCount[item]++;
    }

    private double GetItemUnitPrice(Item item)
    {
        return _rules[item].UnitPrice;
    }

    private bool EligibleForDiscount(Item item)
    {
        if (!ItemOnSpecial(item))
            return false;

        var scannedItemQuantity = _scannedItemsCount[item];
        var quantityEligibleForDiscount = _rules[item].Special!.Quantity;
        var itemQuantityMatchesSpecial = scannedItemQuantity % quantityEligibleForDiscount == 0;

        return itemQuantityMatchesSpecial;
    }

    private bool ItemOnSpecial(Item item)
    {
        return _rules[item].Special != null;
    }

    private double GetDiscountedPrice(Item item)
    {
        var specialPrice = GetItemsSpecialPrice(item);

        return -1 * GetPriceAccountedForInTotal(item) + specialPrice;
    }

    private double GetItemsSpecialPrice(Item item)
    {
        return _rules[item].Special!.Price;
    }

    private double GetPriceAccountedForInTotal(Item item)
    {
        return _rules[item].UnitPrice * (_rules[item].Special!.Quantity - 1);
    }
}
