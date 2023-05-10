using Checkout.Models;

namespace Checkout.Exceptions;

public class InvalidWeightException : Exception
{
    public InvalidWeightException(Item item, double weight)
        : base($"Invalid weight: {weight} for {item}.")
    { }
}