namespace darts.Core.Expression;

public class SimpleMathInterpreter
{
    public (bool Success, int? Value) Resolve(string expression)
    {
        try
        {
            var root = new AdditionNode();
            foreach (var s in expression.Trim().Split('+'))
            {
                var productNode = new ProductNode();
                foreach (var p in s.Split('*'))
                {
                    productNode.Children.Add(new ValueNode(int.Parse(p)));
                }
                root.Children.Add(productNode);
            }
            return (true, root.Resolve());
        }
        catch
        {
            return (false, null);
        }
    }
}
