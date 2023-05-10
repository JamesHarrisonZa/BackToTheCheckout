using Checkout.Models;

namespace Checkout.Exceptions;

public class UnexpectedItemException : Exception
{
    public UnexpectedItemException(Item item)
        : base($"Unexpected Item: {item}. Missing from pricing rules.")
    { }
}