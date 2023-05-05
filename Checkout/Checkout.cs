namespace Checkout;

public class Checkout
{
    public double Total { get; internal set; }

    private IDictionary<string, double> _rules;

	public Checkout(IDictionary<string, double> rules)
	{
		this._rules = rules;
	}

	public void Scan(string item)
	{
		var itemUnitPrice = _rules[item];

        Total += itemUnitPrice;
	}
}
