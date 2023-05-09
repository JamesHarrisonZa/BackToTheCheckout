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
    public double UnitPrice { get; internal set; }
    public bool isPricedByWeight { get; internal set; }

    public Special? Special { get; internal set; }

    public ItemPrice(double unitPrice, Special? special = null)
    {
        UnitPrice = unitPrice;
        Special = special;
    }
}

public class Special
{
    public int Quantity { get; internal set; }

    public double Price { get; internal set; }

    public Special(int quantity, double price)
    {
        Quantity = quantity;
        Price = price;
    }
}