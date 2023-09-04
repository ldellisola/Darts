namespace Darts.Core.Expression;

public abstract class Node
{
    public List<Node> Children { get; } = new();

    public abstract int Resolve();

}

public class ValueNode : Node
{
    private int Value { get; }

    public ValueNode(int value)
    {
        Value = value;
    }

    public override int Resolve()
    {
        return Value;
    }
}


public class AdditionNode : Node
{
    public override int Resolve()
    {
        return Children.Sum(c => c.Resolve());
    }
}

public class ProductNode : Node
{
    public override int Resolve()
    {
        return Children.Aggregate(1, (current, child) => current * child.Resolve());
    }
}
