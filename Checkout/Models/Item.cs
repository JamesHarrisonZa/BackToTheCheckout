namespace Checkout.Models;

public enum Item
{
    A,
    B,
    C,
    D
};

public class ItemPrice
{
    public ItemPrice(double unitPrice, Special? special = null)
    {
        UnitPrice = unitPrice;
        Special = special;
    }

    public double UnitPrice { get; internal set; }

    public Special? Special { get; internal set; }
}

public class Special
{
    public int Quantity { get; internal set; }

    public double SpecialPrice { get; internal set; }
}