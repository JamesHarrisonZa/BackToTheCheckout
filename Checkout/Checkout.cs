using Checkout.Models;
using Checkout.Exceptions;

namespace Checkout;

public class Checkout
{
    public double Total => CalculateTotal();

    private readonly IDictionary<Item, ItemPrice> _rules;

    private readonly IDictionary<Item, int> _scannedItemsCount;  //PricePerItem
    private readonly IDictionary<Item, double> _scannedItemsWeight; //PricePerWeight

    private const int RoundingPrecision = 2;

    public Checkout(IDictionary<Item, ItemPrice> rules)
    {
        _rules = rules;
        _scannedItemsCount = new Dictionary<Item, int>();
        _scannedItemsWeight = new Dictionary<Item, double>();
    }

    public void Scan(Item item)
    {
        AddToScannedItems(item);
    }

    public void Scan(Item item, double weight)
    {
        AddToScannedItems(item, weight);
    }

    private double CalculateTotal()
    {
        var total = 0d;

        foreach (var (item, itemQty) in _scannedItemsCount)
        {
            if (IsOnSpecial(item))
                total += GetItemTotalWithSpecial(item, itemQty);
            else
                total += GetItemTotal(item, itemQty);
        }

        foreach (var (item, weight) in _scannedItemsWeight)
        {
            if (IsOnSpecial(item))
                total += GetItemTotalWithSpecial(item, weight);
            else
                total += GetItemTotal(item, weight);
        }

        return total;
    }

    private void AddToScannedItems(Item item)
    {
        ValidateItemPriceRule(item);

        if (!_scannedItemsCount.ContainsKey(item))
            _scannedItemsCount.Add(item, 0);

        _scannedItemsCount[item]++;
    }

    private void AddToScannedItems(Item item, double weight)
    {
        ValidateItemPriceRule(item);

        if (!_scannedItemsWeight.ContainsKey(item))
            _scannedItemsWeight.Add(item, 0);

        _scannedItemsWeight[item]+= weight;
    }

    private void ValidateItemPriceRule(Item item)
    {
        if (!_rules.ContainsKey(item))
            throw new UnexpectedItemException(item);
    }

    private double GetItemTotalWithSpecial(Item item, int itemQty)
    {
        var unitPrice = _rules[item].UnitPrice;
        var specialQty = _rules[item].Special!.Quantity;
        var specialPrice = _rules[item].Special!.Price;

        var specialCount = itemQty / specialQty;
        var remainingCount = itemQty % specialQty;

        return (specialCount * specialPrice) + (remainingCount * unitPrice);
    }

    private double GetItemTotalWithSpecial(Item item, double weight)
    {
        var unitPrice = _rules[item].UnitPrice;
        var specialWeight = _rules[item].Special!.Weight;
        var specialPrice = _rules[item].Special!.Price;

        var specialCount = (int)(weight / specialWeight);
        var remainingCount = weight % specialWeight;

        return (specialCount * specialPrice) + (remainingCount * unitPrice);
    }

    private double GetItemTotal(Item item, int itemQty)
    {
        return itemQty * _rules[item].UnitPrice;
    }

    private double GetItemTotal(Item item, double itemWeight)
    {
        var itemPrice = itemWeight * _rules[item].UnitPrice;
        var itemPriceRounded = Math.Round(itemPrice, RoundingPrecision);

        return itemPriceRounded;
    }

    private bool IsOnSpecial(Item item)
    {
        return _rules[item].Special != null;
    }
}