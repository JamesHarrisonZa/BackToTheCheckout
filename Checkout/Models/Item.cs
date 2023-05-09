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
    public double UnitPrice { get; }

    public Special? Special { get; }

    public ItemPrice(double unitPrice, Special? special = null)
    {
        UnitPrice = unitPrice;
        Special = special;
    }
}

public class Special
{
    public int Quantity { get; }
    public double Weight { get; }

    public double Price { get; }

    public Special(int quantity, double price)
    {
        Quantity = quantity;
        Price = price;
    }

    public Special(double weight, double price)
    {
        Weight = weight;
        Price = price;
    }
}