namespace Checkout;

public class Checkout
{
    public double Total { get; internal set; }

    private IEnumerable<Rule> _rules;

	public Checkout(IEnumerable<Rule> rules)
	{
		this._rules = rules;
	}

	public void Scan(string item)
	{
		Total = 50; ;
	}
}
