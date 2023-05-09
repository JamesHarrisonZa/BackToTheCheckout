using Checkout.Models;

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



        //if (IsEligibleForDiscount(item))
        //    Total += GetDiscountedPrice(item);
        //else
        //    Total += GetItemUnitPrice(item);
    }

    public void Scan(Item item, double weight)
    {
        AddToScannedItems(item, weight);

        //Total += GetPriceForWeight(item, weight);
    }

    private double CalculateTotal()
    {
        var total = 0d;

        foreach (var (item, itemQty) in _scannedItemsCount)
        {

            if (IsOnSpecial(item))
            {
                var unitPrice = _rules[item].UnitPrice;
                var specialQty = _rules[item].Special!.Quantity;
                var specialPrice = _rules[item].Special!.Price;

                int specialCount = itemQty / specialQty;
                var nonSpecialCount = itemQty % specialQty;

                total += specialCount * specialPrice;
                total += nonSpecialCount * unitPrice;
            }
            else
                total += itemQty * GetItemUnitPrice(item);
        }

        return total;
    }

    private void AddToScannedItems(Item item)
    {
        if (!_scannedItemsCount.ContainsKey(item))
            _scannedItemsCount.Add(item, 0);

        _scannedItemsCount[item]++;
    }

    private void AddToScannedItems(Item item, double weight)
    {
        if (!_scannedItemsWeight.ContainsKey(item))
            _scannedItemsWeight.Add(item, 0);

        _scannedItemsWeight[item]+= weight;
    }

    private double GetItemUnitPrice(Item item)
    {
        return _rules[item].UnitPrice;
    }

    //private bool IsEligibleForDiscount(Item item, int quantity)
    //{
    //    var quantityEligibleForDiscount = _rules[item].Special!.Quantity;
    //    var itemQuantityMatchesSpecial = quantity % quantityEligibleForDiscount == 0;

    //    return itemQuantityMatchesSpecial;
    //}

    private bool IsOnSpecial(Item item)
    {
        return _rules[item].Special != null;
    }

    //private double GetDiscountedPrice(Item item)
    //{
    //    var specialPrice = GetItemsSpecialPrice(item);

    //    return -1 * GetPriceAccountedForInTotal(item) + specialPrice;
    //}

    //private double GetItemsSpecialPrice(Item item)
    //{
    //    return _rules[item].Special!.Price;
    //}

    //private double GetPriceAccountedForInTotal(Item item)
    //{
    //    return _rules[item].UnitPrice * (_rules[item].Special!.Quantity - 1);
    //}

    //private double GetPriceForWeight(Item item, double weight)
    //{
    //    var itemPrice = weight * GetItemUnitPrice(item);
    //    var itemPriceRounded = Math.Round(itemPrice, RoundingPrecision);
    //    return itemPriceRounded;
    //}
}
